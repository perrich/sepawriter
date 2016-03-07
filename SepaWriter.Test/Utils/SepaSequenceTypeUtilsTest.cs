using System;
using NUnit.Framework;
using Perrich.SepaWriter.Utils;

namespace Perrich.SepaWriter.Test.Utils
{
    [TestFixture]
    public class SepaSequenceTypeUtilsTest
    {
        private const string FirstPart = "012345678";

        [Test]
        public void ShouldRetrieveSequenceTypeFromString()
        {
            Assert.AreEqual(SepaSequenceType.OOFF, SepaSequenceTypeUtils.SepaSequenceTypeFromString("OOFF"));
            Assert.AreEqual(SepaSequenceType.FIRST, SepaSequenceTypeUtils.SepaSequenceTypeFromString("FRST"));
            Assert.AreEqual(SepaSequenceType.RCUR, SepaSequenceTypeUtils.SepaSequenceTypeFromString("RCUR"));
            Assert.AreEqual(SepaSequenceType.FINAL, SepaSequenceTypeUtils.SepaSequenceTypeFromString("FNAL"));
        }

        [Test]
        public void ShouldRejectUnknownSequenceType()
        {
            Assert.That(() => { SepaSequenceTypeUtils.SepaSequenceTypeFromString("unknown value"); },
                Throws.TypeOf<ArgumentException>().With.Property("Message").Contains("Unknown Sequence Type"));
            
        }

        [Test]
        public void ShouldRetrieveStringFromSequenceType()
        {
            Assert.AreEqual("OOFF", SepaSequenceTypeUtils.SepaSequenceTypeToString(SepaSequenceType.OOFF));
            Assert.AreEqual("FRST", SepaSequenceTypeUtils.SepaSequenceTypeToString(SepaSequenceType.FIRST));
            Assert.AreEqual("RCUR", SepaSequenceTypeUtils.SepaSequenceTypeToString(SepaSequenceType.RCUR));
            Assert.AreEqual("FNAL", SepaSequenceTypeUtils.SepaSequenceTypeToString(SepaSequenceType.FINAL));
        }
    }
}