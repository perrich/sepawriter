using System;
using System.Xml;

namespace Perrich.SepaWriter.Utils
{
    public static class XmlElementExtension
    {
        public static XmlElement NewElement(this XmlElement parent, string name)
        {
            return NewElement(parent, name, null);
        }

        public static XmlElement NewElement(this XmlElement parent, string name, object value)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            if (parent.OwnerDocument == null)
                throw new ArgumentException("parent hasn't OwnerDocument!");

            XmlElement e = parent.OwnerDocument.CreateElement(name);
            if (value != null)
                e.InnerText = value.ToString();
            parent.AppendChild(e);
            return e;
        }
    }
}