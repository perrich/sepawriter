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
    }
}
