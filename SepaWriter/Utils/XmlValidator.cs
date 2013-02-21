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
    ///     This class is not thread safe.
    /// </summary>
    public class XmlValidator
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (XmlValidator));

        /// <summary>
        ///     XML Validator for Sepa Credit Transfert. Uses XSD in http://www.iso20022.org/documents/messages/pain/schemas/pain.001.001.03.zip
        ///     (see http://www.iso20022.org/full_catalogue.page)
        /// </summary>
        public static XmlValidator SepaCreditTransferValidator = new XmlValidator("Perrich.SepaWriter.Xsd.pain.001.001.03.xsd");

        private readonly XmlSchema _xSchema;
        private bool _result;

        private XmlValidator(string ressourceName)
        {
            var strmrStreamReader =
                new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(ressourceName));
            _xSchema = XmlSchema.Read(strmrStreamReader, null);
        }

        public bool Validate(XmlDocument document)
        {
            if (document == null) return false;
            return Validate(document.OuterXml);
        }

        public bool Validate(String xml)
        {
            if (String.IsNullOrEmpty(xml)) return false;

            _result = true;
            try
            {
                var xmlSettings = new XmlReaderSettings();
                xmlSettings.Schemas = new XmlSchemaSet();
                xmlSettings.Schemas.Add(_xSchema);
                xmlSettings.ValidationType = ValidationType.Schema;
                xmlSettings.ValidationEventHandler += ValidationEventHandler;

                using (XmlReader reader = XmlReader.Create(new StringReader(xml), xmlSettings))
                {
                    while (reader.Read())
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Validation issue due to an exception", ex);
                _result = false;
            }

            return _result;
        }

        private void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Error || e.Severity == XmlSeverityType.Warning)
            {
                _result = false;
                Log.ErrorFormat("Validation issue at line: {0}, position: {1} \"{2}\"",
                                e.Exception.LineNumber, e.Exception.LinePosition,
                                e.Exception.Message);
            }
        }
    }
}