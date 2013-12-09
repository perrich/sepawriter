using NUnit.Framework;

namespace Perrich.SepaWriter.Test
{
    [TestFixture]
    public class SepaTransferTransactionTest
    {
        [Test]
        [ExpectedException(typeof (SepaRuleException), ExpectedMessage = "Invalid amount value",
            MatchType = MessageMatch.Contains)]
        public void ShouldRejectAmountGreaterOrEqualsThan1000000000()
        {
            new SepaCreditTransferTransaction {Amount = 1000000000};
        }

        [Test]
        [ExpectedException(typeof (SepaRuleException), ExpectedMessage = "Invalid amount value",
            MatchType = MessageMatch.Contains)]
        public void ShouldRejectAmountLessThan1Cents()
        {
            new SepaCreditTransferTransaction {Amount = 0};
        }

        [Test]
        [ExpectedException(typeof (SepaRuleException), ExpectedMessage = "Amount should have at most 2 decimals",
            MatchType = MessageMatch.Contains)]
        public void ShouldRejectAmountWithMoreThan2Decimals()
        {
            new SepaCreditTransferTransaction {Amount = 12.012m};
        }
    }
}