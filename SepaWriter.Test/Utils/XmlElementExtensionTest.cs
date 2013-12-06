using System.Text;
using System.Xml;
using NUnit.Framework;
using Perrich.SepaWriter.Utils;

namespace Perrich.SepaWriter.Test.Utils
{
    [TestFixture]
    public class XmlElementExtensionTest
    {
        [SetUp]
        public void Prepare()
        {
            document = new XmlDocument();
            document.AppendChild(document.CreateXmlDeclaration("1.0", Encoding.UTF8.BodyName, "yes"));
            element = (XmlElement) document.AppendChild(document.CreateElement("Document"));
        }

        private XmlDocument document;
        private XmlElement element;

        [Test]
        public void ShouldAddMultipleOrderedNewElement()
        {
            const string name = "sample";
            const string name2 = "sample2";
            const string name3 = "sample3";
            var el = element.NewElement(name);
            var el2 = element.NewElement(name2);
            var el3 = element.NewElement(name3);
            Assert.True(element.HasChildNodes);
            Assert.AreEqual(3, element.ChildNodes.Count);
            Assert.AreEqual(el, element.FirstChild);
            Assert.AreEqual(el2, element.ChildNodes[1]);
            Assert.AreEqual(el3, element.LastChild);
        }

        [Test]
        public void ShouldAddNewElementWithAValue()
        {
            const string name = "sample";
            const decimal value = 12.5m;
            var el = element.NewElement(name, value);
            Assert.AreEqual(name, el.Name);
            Assert.AreEqual(value.ToString(), el.InnerText);
            Assert.True(element.HasChildNodes);
            Assert.AreEqual(1, element.ChildNodes.Count);
        }

        [Test]
        public void ShouldAddNewElementWithoutValue()
        {
            const string name = "sample";
            var el = element.NewElement(name);
            Assert.AreEqual(name, el.Name);
            Assert.IsEmpty(el.InnerText);
            Assert.True(element.HasChildNodes);
            Assert.AreEqual(1, element.ChildNodes.Count);
        }

        [Test]
        public void ShouldAddNewElementExplicitlyWithoutValue()
        {
            const string name = "sample";
            var el = element.NewElement(name, null);
            Assert.AreEqual(name, el.Name);
            Assert.IsEmpty(el.InnerText);
            Assert.True(element.HasChildNodes);
            Assert.AreEqual(1, element.ChildNodes.Count);
        }
    }
}