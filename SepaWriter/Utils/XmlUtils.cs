using System;
using System.Xml;

namespace Perrich.SepaWriter.Utils
{
    /// <summary>
    ///     Some Utilities to manage XML
    /// </summary>
    public static class XmlUtils
    {
        /// <summary>
        ///     Find First element in the Xml document with provided name
        /// </summary>
        /// <param name="document">The Xml Document</param>
        /// <param name="nodeName">The name of the node</param>
        /// <returns></returns>
        public static XmlElement GetFirstElement(XmlNode document, string nodeName)
        {
            return document.SelectSingleNode("//" + nodeName) as XmlElement; 
        }
        /// <summary>
        ///     Create a BIC
        /// </summary>
        /// <param name="element">The Xml element</param>
        /// <param name="iban">The iban</param>
        /// <returns></returns>
        public static void CreateBic(XmlElement element, SepaIbanData iban)
        {
            if (iban.UnknownBic)
            {
                element.NewElement("FinInstnId").NewElement("Othr").NewElement("Id", "NOTPROVIDED");
            }
            else
            {
                var finInstnId = element.NewElement("FinInstnId");
                finInstnId.NewElement("BIC", iban.Bic);

                if (iban.AgentAddress != null)
                {
                    AddPostalAddressElements(finInstnId, iban.AgentAddress);
                }
            }

        }

        /// <summary>
        /// Create an XmlElement for the provided address and add to the parent XmlElement
        /// according to the provided key.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="existingElementKey"></param>
        /// <param name="address"></param>
        public static void AddPostalAddressElements(XmlElement parent, SepaPostalAddress address)
        {
            var pstlAdr = parent.NewElement("PstlAdr");
            if (address.AddressType.HasValue)
                pstlAdr.NewElement("AdrTp", address.AddressType.ToString());
            if (!String.IsNullOrEmpty(address.Dept))
                pstlAdr.NewElement("Dept", address.Dept);
            if (!String.IsNullOrEmpty(address.SubDept))
                pstlAdr.NewElement("SubDept", address.SubDept);
            if (!String.IsNullOrEmpty(address.StrtNm))
                pstlAdr.NewElement("StrtNm", address.StrtNm);
            if (!String.IsNullOrEmpty(address.BldgNb))
                pstlAdr.NewElement("BldgNb", address.BldgNb);
            if (!String.IsNullOrEmpty(address.PstCd))
                pstlAdr.NewElement("PstCd", address.PstCd);
            if (!String.IsNullOrEmpty(address.TwnNm))
                pstlAdr.NewElement("TwnNm", address.TwnNm);
            if (!String.IsNullOrEmpty(address.CtrySubDvsn))
                pstlAdr.NewElement("CtrySubDvsn", address.CtrySubDvsn);
            if (!String.IsNullOrEmpty(address.Ctry))
                pstlAdr.NewElement("Ctry", address.Ctry);
            if (address.AdrLine != null)
                foreach (var line in address.AdrLine)
                    pstlAdr.NewElement("AdrLine", line);
        }
    }
}
