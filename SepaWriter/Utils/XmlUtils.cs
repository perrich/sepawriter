using System;
using System.Globalization;
using System.Xml;

namespace Perrich.SepaWriter.Utils
{
    public static class XmlUtils
    {
        /// <summary>
        ///     Format an amount in requested string format
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static string FormatAmount(decimal amount)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0:0.##}", amount);
        }

        /// <summary>
        /// Find First element in the Xml document with provided name
        /// </summary>
        /// <param name="document"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XmlElement GetFirstElement(XmlDocument document, string name)
        {
            return document.SelectSingleNode("//" + name) as XmlElement;
        }
    }
}
