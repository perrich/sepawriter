using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using log4net;

namespace Perrich.SepaWriter.Utils
{
    /// <summary>
    ///     Allow to validate XML against the known XSD.
    ///     This class is not thread safe !
    /// </summary>
    public class XmlValidator
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(XmlValidator));

        /// <summary>
        ///     XML Validator for Sepa Credit Transfert. Uses XSD in http://www.iso20022.org/documents/messages/pain/schemas/pain.001.001.03.zip
        ///     (see http://www.iso20022.org/full_catalogue.page)
        /// </summary>
        public static XmlValidator SepaCreditTransferValidator { get; private set; }

        /// <summary>
        ///     XML Validator for Sepa Credit Transfert. Uses XSD in http://www.iso20022.org/documents/messages/pain/schemas/pain.008.001.02.zip
        ///     (see http://www.iso20022.org/full_catalogue.page)
        /// </summary>
        public static XmlValidator SepaDebitTransferValidator { get; private set; }

        private readonly XmlSchema xmlSchema;
        private bool result;

        static XmlValidator()
        {
            SepaCreditTransferValidator = new XmlValidator("Perrich.SepaWriter.Xsd.pain.001.001.03.xsd");
            SepaDebitTransferValidator = new XmlValidator("Perrich.SepaWriter.Xsd.pain.008.001.02.xsd");
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
                Log.Error("Validation issue due to an exception", ex);
                result = false;
            }

            return result;
        }

        private void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity != XmlSeverityType.Error && e.Severity != XmlSeverityType.Warning) return;

            result = false;
            Log.ErrorFormat("Validation issue at line: {0}, position: {1} \"{2}\"", 
                e.Exception.LineNumber, e.Exception.LinePosition, 
                e.Exception.Message);
        }
    }
}