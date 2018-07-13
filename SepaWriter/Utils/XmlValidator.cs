using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace Perrich.SepaWriter.Utils
{
    /// <summary>
    ///     Allow to validate XML against the known XSD.
    ///     This class is not thread safe !
    /// </summary>
    public class XmlValidator
    {
        private static readonly Dictionary<SepaSchema, XmlValidator> validators = new Dictionary<SepaSchema, XmlValidator>();

        /// <summary>
        /// Get a validator for the provided schema
        /// </summary>
        /// <param name="schema">The schema</param>
        /// <returns>The validator</returns>
        public static XmlValidator GetValidator(SepaSchema schema)
        {
            if (!validators.ContainsKey(schema))
                throw new KeyNotFoundException("Validator for " + schema + " schema not found!");
            return validators[schema];
        }

        private readonly XmlSchema xmlSchema;
        private bool result;

        /// <summary>
        /// Init all available validators (see http://www.iso20022.org/full_catalogue.page) using embedded XSD:
        /// http://www.iso20022.org/documents/messages/pain/schemas/pain.001.001.03.zip
        /// http://www.iso20022.org/documents/messages/pain/schemas/pain.008.001.02.zip
        /// ...    
        /// </summary>
        static XmlValidator()
        {
            foreach (SepaSchema schema in Enum.GetValues(typeof(SepaSchema)))
            {
                validators.Add(schema, new XmlValidator("Perrich.SepaWriter.Xsd." + SepaSchemaUtils.SepaSchemaToString(schema) + ".xsd"));
            }        
        }

        /// <summary>
        /// Create an XML Validator using a internal XSD 
        /// </summary>
        /// <param name="ressourceName">The ressource Name in the executing assembly</param>
        private XmlValidator(string ressourceName)
        {
            using (var strmrStreamReader =
                new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(ressourceName)))
            {
                xmlSchema = XmlSchema.Read(strmrStreamReader, null);
            }
        }

        /// <summary>
        /// Validate an XML Node
        /// </summary>
        /// <param name="node">The XML node</param>
        /// <returns></returns>
        public bool Validate(XmlNode node)
        {
            if (node == null) return false;
            return Validate(node.OuterXml);
        }

        /// <summary>
        /// Validate the XML string
        /// </summary>
        /// <param name="xml">The string</param>
        /// <returns></returns>
        public bool Validate(string xml)
        {
            if (string.IsNullOrEmpty(xml)) return false;

            result = true;
            try
            {
                var xmlSettings = new XmlReaderSettings();
                xmlSettings.Schemas = new XmlSchemaSet();
                xmlSettings.Schemas.Add(xmlSchema);
                xmlSettings.ValidationType = ValidationType.Schema;
                xmlSettings.ValidationEventHandler += ValidationEventHandler;

                using (var reader = XmlReader.Create(new StringReader(xml), xmlSettings))
                {
                    while (reader.Read()); // Read all file
                }
            }
            catch (Exception ex)
            {
                //Log.Error("Validation issue due to an exception", ex);
                result = false;
            }

            return result;
        }

        private void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity != XmlSeverityType.Error && e.Severity != XmlSeverityType.Warning) return;

            result = false;
            //Log.ErrorFormat("Validation issue at line: {0}, position: {1} \"{2}\"", 
            //    e.Exception.LineNumber, e.Exception.LinePosition, 
            //    e.Exception.Message);
        }
    }
}