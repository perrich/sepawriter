using System;
using System.Xml;

namespace Perrich.SepaWriter.Utils
{
    /// <summary>
    /// Extend XmlElement to allow a fluent and easier management
    /// </summary>
    public static class XmlElementExtension
    {
        /// <summary>
        /// Create a new XML Element
        /// </summary>
        /// <param name="parent">The parent node</param>
        /// <param name="name">The new element name</param>
        /// <returns></returns>
        public static XmlElement NewElement(this XmlElement parent, string name)
        {
            return NewElement(parent, name, null);
        }

        /// <summary>
        /// Create a new XML Element
        /// </summary>
        /// <param name="parent">The parent node</param>
        /// <param name="name">The new element name</param>
        /// <param name="value">The new element value</param>
        /// <returns></returns>
        public static XmlElement NewElement(this XmlElement parent, string name, object value)
        {
            var e = parent.OwnerDocument.CreateElement(name);
            if (value != null)
                e.InnerText = value.ToString();
            parent.AppendChild(e);
            return e;
        }
    }
}