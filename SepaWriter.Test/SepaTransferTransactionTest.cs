using NUnit.Framework;

namespace SepaWriter.Test
{
    [TestFixture]
    public class SepaTransferTransactionTest
    {
        [Test]
        public void ShouldRejectAmountGreaterOrEqualsThan1000000000()
        {
            Assert.That(() => { new SepaCreditTransferTransaction { Amount = 1000000000 }; },
                Throws.TypeOf<SepaRuleException>().With.Property("Message").Contains("Invalid amount value"));            
        }

        [Test]
        public void ShouldRejectAmountLessThan1Cents()
        {
            Assert.That(() => { new SepaCreditTransferTransaction { Amount = 0 }; },
                Throws.TypeOf<SepaRuleException>().With.Property("Message").Contains("Invalid amount value"));
        }

        [Test]
        public void ShouldRejectAmountWithMoreThan2Decimals()
        {
            Assert.That(() => { new SepaCreditTransferTransaction { Amount = 12.012m }; },
                Throws.TypeOf<SepaRuleException>().With.Property("Message").Contains("Amount should have at most 2 decimals"));
        }

        [Test]
        public void ShouldRejectEndToEndIdGreaterThan35()
        {
            Assert.That(() => { new SepaCreditTransferTransaction { EndToEndId = "012345678901234567890123456789012345" }; },
                Throws.TypeOf<SepaRuleException>().With.Property("Message").Contains("cannot be greater than 35"));
        }
    }
}