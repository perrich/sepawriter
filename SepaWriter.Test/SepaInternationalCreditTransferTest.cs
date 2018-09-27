using System;
using NUnit.Framework;

namespace Perrich.SepaWriter.Test
{
    [TestFixture]
    public class SepaInternationalCreditTransferTest
    {
        private const string RESULT = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?><Document xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:iso:std:iso:20022:tech:xsd:pain.001.001.03\"><CstmrCdtTrfInitn><GrpHdr><MsgId>REF/789456/CCT001</MsgId><CreDtTm>2010-02-20T09:30:05</CreDtTm><NbOfTxs>2</NbOfTxs><CtrlSum>1520000</CtrlSum><InitgPty><Nm>TOTO Distribution SA</Nm></InitgPty></GrpHdr><PmtInf><PmtInfId>LOT123456</PmtInfId><PmtMtd>TRF</PmtMtd><NbOfTxs>2</NbOfTxs><CtrlSum>1520000</CtrlSum><PmtTpInf><InstrPrty>NORM</InstrPrty></PmtTpInf><ReqdExctnDt>2010-02-28</ReqdExctnDt><Dbtr><Nm>Societe S</Nm></Dbtr><DbtrAcct><Id><IBAN>FR7621479632145698745632145</IBAN></Id><Ccy>EUR</Ccy></DbtrAcct><DbtrAgt><FinInstnId><BIC>BANKFRPP</BIC></FinInstnId></DbtrAgt><ChrgBr>DEBT</ChrgBr><CdtTrfTxInf><PmtId><InstrId>Virement 458A</InstrId><EndToEndId>SOC/1478/CC/TI001/01</EndToEndId></PmtId><Amt><InstdAmt Ccy=\"USD\">20000</InstdAmt></Amt><CdtrAgt><FinInstnId><BIC>PNPBUS33</BIC></FinInstnId></CdtrAgt><Cdtr><Nm>USA Factory</Nm></Cdtr><CdtrAcct><Id><IBAN>US29NWBK60161331926819</IBAN></Id></CdtrAcct><InstrForCdtrAgt><Cd>PHOB</Cd></InstrForCdtrAgt><Purp><Cd>SCVE</Cd></Purp><RgltryRptg><Dtls><Cd>150</Cd></Dtls></RgltryRptg><RmtInf><Ustrd>En reglement des factures numeros : 123456789 987456321 258741369</Ustrd></RmtInf></CdtTrfTxInf><CdtTrfTxInf><PmtId><InstrId>Virement 458B</InstrId><EndToEndId>SOC/1478/CC/TI001/02</EndToEndId></PmtId><Amt><InstdAmt Ccy=\"JPY\">1500000</InstdAmt></Amt><CdtrAgt><FinInstnId><BIC>BANKDEFF</BIC></FinInstnId></CdtrAgt><Cdtr><Nm>Japan Society</Nm></Cdtr><CdtrAcct><Id><IBAN>DE89370400440532013000</IBAN></Id></CdtrAcct><Purp><Cd>SCVE</Cd></Purp><RgltryRptg><Dtls><Cd>150</Cd></Dtls></RgltryRptg><RmtInf><Ustrd>En reglement des factures numeros : 321456789A 789456321B  852741370C</Ustrd></RmtInf></CdtTrfTxInf></PmtInf></CstmrCdtTrfInitn></Document>";

        [Test]
        public void ShouldManageMultipleTransactionsTransfer()
        {
            var transfert = new SepaCreditTransfer
            {
                CreationDate = new DateTime(2010, 02, 20, 9, 30, 5),
                RequestedExecutionDate = new DateTime(2010, 02, 28),
                MessageIdentification = "REF/789456/CCT001",
                PaymentInfoId = "LOT123456",
                InitiatingParty = new InitiatingParty() { Name = "TOTO Distribution SA" },
                Debtor = new SepaIbanData
                {
                    Bic = "BANKFRPP",
                    Iban = "FR7621479632145698745632145",
                    Name = "Societe S"
                },
                IsInternational = true,
                ChargeBearer = SepaChargeBearer.DEBT,
           };

            const decimal amount = 20000m;
            var trans = new SepaCreditTransferTransaction
            {
                Id = "Virement 458A",
                Creditor = new SepaIbanData
                {
                    Bic = "PNPBUS33",
                    Iban = "US29NWBK60161331926819",
                    Name = "USA Factory"
                },
                Amount = amount,
                Currency = "USD",
                EndToEndId = "SOC/1478/CC/TI001/01",
                RemittanceInformation = "En reglement des factures numeros : 123456789 987456321 258741369",
                SepaInstructionForCreditor = new SepaInstructionForCreditor() { Code = SepaInstructionForCreditor.SepaInstructionForCreditorCode.PHOB },
                Purpose = "SCVE",
                RegulatoryReportingCode = "150"
            };
            trans.EndToEndId = "SOC/1478/CC/TI001/01";
            transfert.AddCreditTransfer(trans);

            const decimal amount2 = 1500000m;
            trans = new SepaCreditTransferTransaction
            {
                Id = "Virement 458B",
                Creditor = new SepaIbanData
                {
                    Bic = "BANKDEFF",
                    Iban = "DE89370400440532013000",
                    Name = "Japan Society"
                },
                Amount = amount2,
                Currency = "JPY",
                EndToEndId = "SOC/1478/CC/TI001/02",
                RemittanceInformation = "En reglement des factures numeros : 321456789A 789456321B  852741370C",
                Purpose = "SCVE",
                RegulatoryReportingCode = "150"
            };
            transfert.AddCreditTransfer(trans);

            const decimal total = (amount + amount2)*100;

            Assert.AreEqual(total, transfert.HeaderControlSumInCents);
            Assert.AreEqual(total, transfert.PaymentControlSumInCents);

            Assert.AreEqual(RESULT, transfert.AsXmlString());
        }
    }
}