using System;
using NUnit.Framework;

namespace SepaWriter.Test
{
    [TestFixture]
    public class SepaDebitTransferTransactionTest
    {
        private const string Bic = "SOGEFRPPXXX";
        private const string Iban = "FR7030002005500000157845Z02";
        private const string Name = "A_NAME";

        private readonly SepaIbanData iBanData = new SepaIbanData
            {
                Bic = Bic,
                Iban = Iban,
                Name = Name
            };

        [Test]
        public void ShouldHaveADefaultCurrency()
        {
            var data = new SepaDebitTransferTransaction();

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
            const string mandateId = "MyMandate";
            var signatureDate = new DateTime(2012, 12, 2);
            var seqType = SepaSequenceType.FIRST;

            var data = new SepaDebitTransferTransaction
                {
                    Debtor = iBanData,
                    Amount = amount,
                    Currency = currency,
                    Id = id,
                    EndToEndId = endToEndId,
                    RemittanceInformation = remittanceInformation,
                    DateOfSignature = signatureDate,
                    MandateIdentification = mandateId,
                    SequenceType = SepaSequenceType.FIRST
                };

            Assert.AreEqual(currency, data.Currency);
            Assert.AreEqual(amount, data.Amount);
            Assert.AreEqual(id, data.Id);
            Assert.AreEqual(endToEndId, data.EndToEndId);
            Assert.AreEqual(remittanceInformation, data.RemittanceInformation);
            Assert.AreEqual(Bic, data.Debtor.Bic);
            Assert.AreEqual(Iban, data.Debtor.Iban);
            Assert.AreEqual(Iban, data.Debtor.Iban);
            Assert.AreEqual(mandateId, data.MandateIdentification);
            Assert.AreEqual(signatureDate, data.DateOfSignature);
            Assert.AreEqual(seqType, data.SequenceType);

            var data2 = data.Clone() as SepaDebitTransferTransaction;

            Assert.NotNull(data2);
            Assert.AreEqual(currency, data2.Currency);
            Assert.AreEqual(amount, data2.Amount);
            Assert.AreEqual(id, data2.Id);
            Assert.AreEqual(endToEndId, data2.EndToEndId);
            Assert.AreEqual(remittanceInformation, data2.RemittanceInformation);
            Assert.AreEqual(Bic, data2.Debtor.Bic);
            Assert.AreEqual(Iban, data2.Debtor.Iban);
            Assert.AreEqual(mandateId, data2.MandateIdentification);
            Assert.AreEqual(signatureDate, data2.DateOfSignature);
            Assert.AreEqual(seqType, data2.SequenceType);
        }

        [Test]
        public void ShouldRejectInvalidDebtor()
        {
            Assert.That(() => { new SepaDebitTransferTransaction { Debtor = new SepaIbanData() }; },
                Throws.TypeOf<SepaRuleException>().With.Property("Message").Contains("Debtor IBAN data are invalid."));
        }

        [Test]
        public void ShouldUseADefaultSequenceType()
        {
            var transfert = new SepaDebitTransferTransaction();
            Assert.AreEqual(SepaSequenceType.OOFF, transfert.SequenceType);
        }
    }
}