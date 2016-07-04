using NUnit.Framework;
using Perrich.SepaWriter.Utils;
using System;

namespace Perrich.SepaWriter.Test.Utils
{
    [TestFixture]
    public class SepaChargeBearerUtilsTest
    {
        [Test]
        public void ShouldRetrieveChargeBearerFromString()
        {
            Assert.AreEqual(SepaChargeBearer.CRED,  SepaChargeBearerUtils.SepaChargeBearerFromString("CRED"));
            Assert.AreEqual(SepaChargeBearer.DEBT, SepaChargeBearerUtils.SepaChargeBearerFromString("DEBT"));
            Assert.AreEqual(SepaChargeBearer.SHAR,  SepaChargeBearerUtils.SepaChargeBearerFromString("SHAR"));
        }

        [Test]
        public void ShouldRejectUnknownChargeBearer()
        {
            Assert.That(() => { SepaChargeBearerUtils.SepaChargeBearerFromString("unknown value"); },
                Throws.TypeOf<ArgumentException>().With.Property("Message").Contains("Unknown Charge Bearer"));
            
        }

        [Test]
        public void ShouldRetrieveStringFromChargeBearer()
        {
            Assert.AreEqual("CRED", SepaChargeBearerUtils.SepaChargeBearerToString(SepaChargeBearer.CRED));
            Assert.AreEqual("DEBT", SepaChargeBearerUtils.SepaChargeBearerToString(SepaChargeBearer.DEBT));
            Assert.AreEqual("SHAR", SepaChargeBearerUtils.SepaChargeBearerToString(SepaChargeBearer.SHAR));
        }
    }
}