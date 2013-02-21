using NUnit.Framework;

namespace Perrich.SepaWriter.Test
{
    [TestFixture]
    public class SepaCreditTransferTransactionTest
    {
        private const string Bic = "BANK_BIC";
        private const string Iban = "ACCOUNT_IBAN";
        private const string Name = "NAME";

        private readonly SepaIbanData _iBanData = new SepaIbanData
            {
                Bic = Bic,
                Iban = Iban,
                Name = Name
            };

        [Test]
        public void ShouldHaveEuroAsDefaultCurrency()
        {
            var data = new SepaCreditTransferTransaction();

            Assert.AreEqual("EUR", data.Currency);
        }

        [Test]
        public void ShouldKeepProvidedData()
        {
            const decimal amount = 100m;
            const string currency = "USD";
            const string id = "Batch1";
            const string endToEndId = "Batch1/Row2";
            const string remittanceInformation = "Sample";

            var data = new SepaCreditTransferTransaction
                {
                    Creditor = _iBanData,
                    Amount = amount,
                    Currency = currency,
                    Id = id,
                    EndToEndId = endToEndId,
                    RemittanceInformation = remittanceInformation
                };

            Assert.AreEqual(currency, data.Currency);
            Assert.AreEqual(amount, data.Amount);
            Assert.AreEqual(id, data.Id);
            Assert.AreEqual(endToEndId, data.EndToEndId);
            Assert.AreEqual(remittanceInformation, data.RemittanceInformation);
            Assert.AreEqual(Bic, data.Creditor.Bic);
            Assert.AreEqual(Iban, data.Creditor.Iban);

            var data2 = data.Clone() as SepaCreditTransferTransaction;

            Assert.NotNull(data2);
            Assert.AreEqual(currency, data2.Currency);
            Assert.AreEqual(amount, data2.Amount);
            Assert.AreEqual(id, data2.Id);
            Assert.AreEqual(endToEndId, data2.EndToEndId);
            Assert.AreEqual(remittanceInformation, data2.RemittanceInformation);
            Assert.AreEqual(Bic, data2.Creditor.Bic);
            Assert.AreEqual(Iban, data2.Creditor.Iban);
        }

        [Test]
        [ExpectedException(typeof(SepaRuleException), ExpectedMessage = "Invalid amount value",
            MatchType = MessageMatch.Contains)]
        public void ShouldRejectAmountGreaterOrEqualsThan1000000000()
        {
            new SepaCreditTransferTransaction { Amount = 1000000000 };
        }

        [Test]
        [ExpectedException(typeof(SepaRuleException), ExpectedMessage = "Invalid amount value",
            MatchType = MessageMatch.Contains)]
        public void ShouldRejectAmountLessThan1Cents()
        {
            new SepaCreditTransferTransaction { Amount = 0 };
        }

        [Test]
        [ExpectedException(typeof(SepaRuleException), ExpectedMessage = "Amount should have at most 2 decimals",
            MatchType = MessageMatch.Contains)]
        public void ShouldRejectAmountWithMoreThan2Decimals()
        {
            new SepaCreditTransferTransaction { Amount = 12.012m };
        }
    }
}