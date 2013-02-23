using System;
using System.IO;
using System.Xml;
using NUnit.Framework;
using Perrich.SepaWriter.Utils;

namespace Perrich.SepaWriter.Test
{
    [TestFixture]
    public class SepaCreditTransferTest
    {
        private const string FILENAME = "sepa_test_result.xml";

        private const string ONE_ROW_RESULT =
            "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?><Document xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:iso:std:iso:20022:tech:xsd:pain.001.001.03\"><CstmrCdtTrfInitn><GrpHdr><MsgId>transferID</MsgId><CreDtTm>2013-02-17T22:38:12</CreDtTm><NbOfTxs>1</NbOfTxs><CtrlSum>23.45</CtrlSum><InitgPty><Nm>Me</Nm></InitgPty></GrpHdr><PmtInf><PmtInfId>paymentInfo</PmtInfId><PmtMtd>TRF</PmtMtd><NbOfTxs>1</NbOfTxs><CtrlSum>23.45</CtrlSum><PmtTpInf><SvcLvl><Cd>SEPA</Cd></SvcLvl></PmtTpInf><ReqdExctnDt>2013-02-17</ReqdExctnDt><Dbtr><Nm>My Corp</Nm></Dbtr><DbtrAcct><Id><IBAN>FR7030002005500000157845Z02</IBAN></Id><Ccy>EUR</Ccy></DbtrAcct><DbtrAgt><FinInstnId><BIC>SOGEFRPPXXX</BIC></FinInstnId></DbtrAgt><ChrgBr>SLEV</ChrgBr><CdtTrfTxInf><PmtId><InstrId>Transaction Id 1</InstrId><EndToEndId>transferID/1</EndToEndId></PmtId><Amt><InstdAmt Ccy=\"EUR\">23.45</InstdAmt></Amt><CdtrAgt><FinInstnId><BIC>AGRIFRPPXXX</BIC></FinInstnId></CdtrAgt><Cdtr><Nm>THEIR_NAME</Nm></Cdtr><CdtrAcct><Id><IBAN>FR1420041010050500013M02606</IBAN></Id></CdtrAcct><RmtInf><Ustrd>Transaction description</Ustrd></RmtInf></CdtTrfTxInf></PmtInf></CstmrCdtTrfInitn></Document>";

        private const string MULTIPLE_ROW_RESULT =
            "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?><Document xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"urn:iso:std:iso:20022:tech:xsd:pain.001.001.03\"><CstmrCdtTrfInitn><GrpHdr><MsgId>transferID</MsgId><CreDtTm>2013-02-17T22:38:12</CreDtTm><NbOfTxs>3</NbOfTxs><CtrlSum>63.36</CtrlSum><InitgPty><Nm>Me</Nm></InitgPty></GrpHdr><PmtInf><PmtInfId>paymentInfo</PmtInfId><PmtMtd>TRF</PmtMtd><NbOfTxs>3</NbOfTxs><CtrlSum>63.36</CtrlSum><PmtTpInf><SvcLvl><Cd>SEPA</Cd></SvcLvl></PmtTpInf><ReqdExctnDt>2013-02-18</ReqdExctnDt><Dbtr><Nm>My Corp</Nm></Dbtr><DbtrAcct><Id><IBAN>FR7030002005500000157845Z02</IBAN></Id><Ccy>EUR</Ccy></DbtrAcct><DbtrAgt><FinInstnId><BIC>SOGEFRPPXXX</BIC></FinInstnId></DbtrAgt><ChrgBr>SLEV</ChrgBr><CdtTrfTxInf><PmtId><InstrId>Transaction Id 1</InstrId><EndToEndId>multiple1</EndToEndId></PmtId><Amt><InstdAmt Ccy=\"EUR\">23.45</InstdAmt></Amt><CdtrAgt><FinInstnId><BIC>AGRIFRPPXXX</BIC></FinInstnId></CdtrAgt><Cdtr><Nm>THEIR_NAME</Nm></Cdtr><CdtrAcct><Id><IBAN>FR1420041010050500013M02606</IBAN></Id></CdtrAcct><RmtInf><Ustrd>Transaction description</Ustrd></RmtInf></CdtTrfTxInf><CdtTrfTxInf><PmtId><InstrId>Transaction Id 2</InstrId><EndToEndId>transferID/2</EndToEndId></PmtId><Amt><InstdAmt Ccy=\"EUR\">12.56</InstdAmt></Amt><CdtrAgt><FinInstnId><BIC>AGRIFRPPXXX</BIC></FinInstnId></CdtrAgt><Cdtr><Nm>THEIR_NAME</Nm></Cdtr><CdtrAcct><Id><IBAN>FR1420041010050500013M02606</IBAN></Id></CdtrAcct><RmtInf><Ustrd>Transaction description 2</Ustrd></RmtInf></CdtTrfTxInf><CdtTrfTxInf><PmtId><InstrId>Transaction Id 3</InstrId><EndToEndId>transferID/3</EndToEndId></PmtId><Amt><InstdAmt Ccy=\"EUR\">27.35</InstdAmt></Amt><CdtrAgt><FinInstnId><BIC>BANK_BIC</BIC></FinInstnId></CdtrAgt><Cdtr><Nm>NAME</Nm></Cdtr><CdtrAcct><Id><IBAN>ACCOUNT_IBAN</IBAN></Id></CdtrAcct><RmtInf><Ustrd>Transaction description 3</Ustrd></RmtInf></CdtTrfTxInf></PmtInf></CstmrCdtTrfInitn></Document>";

        private static SepaCreditTransfer GetOneTransactionCreditTransfert(decimal amount)
        {
            var transfert = new SepaCreditTransfer
                {
                    CreationDate = new DateTime(2013, 02, 17, 22, 38, 12),
                    RequestedExecutionDate = new DateTime(2013, 02, 17),
                    MessageIdentification = "transferID",
                    PaymentInfoId = "paymentInfo",
                    InitiatingPartyName = "Me",
                    Debtor =
                        new SepaIbanData {Bic = "SOGEFRPPXXX", Iban = "FR7030002005500000157845Z02", Name = "My Corp"}
                };

            transfert.AddCreditTransfer("Transaction Id 1", null,
                                        new SepaIbanData
                                            {
                                                Bic = "AGRIFRPPXXX",
                                                Iban = "FR1420041010050500013M02606",
                                                Name = "THEIR_NAME"
                                            }, amount, "Transaction description");
            return transfert;
        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
            if (File.Exists(FILENAME))
                File.Delete(FILENAME);
        }

        [Test]
        public void ShouldAllowMultipleNullIdTransations()
        {
            const decimal amount = 23.45m;

            SepaCreditTransfer transfert = GetOneTransactionCreditTransfert(amount);

            transfert.AddCreditTransfer(new SepaCreditTransferTransaction
                {
                    Id = null,
                    Creditor = new SepaIbanData
                        {
                            Bic = "AGRIFRPPXXX",
                            Iban = "FR1420041010050500013M02606",
                            Name = "THEIR_NAME"
                        },
                    Amount = amount,
                    RemittanceInformation = "Transaction description 1"
                });

            transfert.AddCreditTransfer(new SepaCreditTransferTransaction
                {
                    Id = null,
                    Creditor = new SepaIbanData
                        {
                            Bic = "AGRIFRPPXXX",
                            Iban = "FR1420041010050500013M02606",
                            Name = "THEIR_NAME"
                        },
                    Amount = amount,
                    RemittanceInformation = "Transaction description 2"
                });
        }

        [Test]
        public void ShouldKeepEndToEndIdIfSet()
        {
            const decimal amount = 23.45m;

            SepaCreditTransfer transfert = GetOneTransactionCreditTransfert(amount);

            transfert.AddCreditTransfer(new SepaCreditTransferTransaction
            {
                Id = null,
                EndToEndId = "endToendId1",
                Creditor = new SepaIbanData
                {
                    Bic = "AGRIFRPPXXX",
                    Iban = "FR1420041010050500013M02606",
                    Name = "THEIR_NAME"
                },
                Amount = amount,
                RemittanceInformation = "Transaction description 2"
            });

            transfert.AddCreditTransfer(null, "endToendId2",
                                        new SepaIbanData
                                        {
                                            Bic = "AGRIFRPPXXX",
                                            Iban = "FR1420041010050500013M02606",
                                            Name = "THEIR_NAME"
                                        },
                                        amount, "EUR", "Transaction description 3");

            var result = transfert.AsXmlString();

            Assert.True(result.Contains("<EndToEndId>endToendId1</EndToEndId>"));
            Assert.True(result.Contains("<EndToEndId>endToendId2</EndToEndId>"));
        }

        [Test]
        public void ShouldManageMultipleTransactionsTransfer()
        {
            var transfert = new SepaCreditTransfer
                {
                    CreationDate = new DateTime(2013, 02, 17, 22, 38, 12),
                    RequestedExecutionDate = new DateTime(2013, 02, 18),
                    MessageIdentification = "transferID",
                    PaymentInfoId = "paymentInfo",
                    InitiatingPartyName = "Me",
                    Debtor =
                        new SepaIbanData {Bic = "SOGEFRPPXXX", Iban = "FR7030002005500000157845Z02", Name = "My Corp"}
                };

            const decimal amount = 23.45m;

            transfert.AddCreditTransfer("Transaction Id 1", "multiple1",
                                        new SepaIbanData
                                            {
                                                Bic = "AGRIFRPPXXX",
                                                Iban = "FR1420041010050500013M02606",
                                                Name = "THEIR_NAME"
                                            },
                                        amount, "EUR", "Transaction description");
            const decimal amount2 = 12.56m;

            transfert.AddCreditTransfer(new SepaCreditTransferTransaction
                {
                    Id = "Transaction Id 2",
                    Creditor = new SepaIbanData
                        {
                            Bic = "AGRIFRPPXXX",
                            Iban = "FR1420041010050500013M02606",
                            Name = "THEIR_NAME"
                        },
                    Amount = amount2,
                    RemittanceInformation = "Transaction description 2"
                });

            const decimal amount3 = 27.35m;

            transfert.AddCreditTransfer("Transaction Id 3", null,
                                        new SepaIbanData
                                            {
                                                Bic = "BANK_BIC",
                                                Iban = "ACCOUNT_IBAN",
                                                Name = "NAME"
                                            },
                                        amount3, "Transaction description 3");

            const decimal total = (amount + amount2 + amount3)*100;

            Assert.AreEqual(total, transfert.GetHeaderControlSumInCents());
            Assert.AreEqual(total, transfert.GetPaymentControlSumInCents());

            Assert.AreEqual(MULTIPLE_ROW_RESULT, transfert.AsXmlString());

            XmlValidator validator = XmlValidator.SepaCreditTransferValidator;
            validator.Validate(transfert.AsXmlString());
        }

        [Test]
        public void ShouldManageOneTransactionTransfer()
        {
            const decimal amount = 23.45m;
            SepaCreditTransfer transfert = GetOneTransactionCreditTransfert(amount);

            const decimal total = amount*100;
            Assert.AreEqual(total, transfert.GetHeaderControlSumInCents());
            Assert.AreEqual(total, transfert.GetPaymentControlSumInCents());

            Assert.AreEqual(ONE_ROW_RESULT, transfert.AsXmlString());
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException), ExpectedMessage = "transfer",
            MatchType = MessageMatch.Contains)]
        public void ShouldRejectNullTransactionTransfer()
        {
            var transfert = new SepaCreditTransfer();

            transfert.AddCreditTransfer(null);
        }

        [Test]
        [ExpectedException(typeof (SepaRuleException), ExpectedMessage = "must be unique in a transfer",
            MatchType = MessageMatch.Contains)]
        public void ShouldRejectTwoTransationsWithSameId()
        {
            const decimal amount = 23.45m;

            SepaCreditTransfer transfert = GetOneTransactionCreditTransfert(amount);

            transfert.AddCreditTransfer(new SepaCreditTransferTransaction
                {
                    Id = "Transaction Id 1",
                    Creditor = new SepaIbanData
                        {
                            Bic = "AGRIFRPPXXX",
                            Iban = "FR1420041010050500013M02606",
                            Name = "THEIR_NAME"
                        },
                    Amount = amount,
                    RemittanceInformation = "Transaction description 2"
                });
        }

        [Test]
        public void ShouldSaveInXmlFile()
        {
            const decimal amount = 23.45m;

            SepaCreditTransfer transfert = GetOneTransactionCreditTransfert(amount);
            transfert.Save(FILENAME);

            var doc = new XmlDocument();
            doc.Load(FILENAME);

            Assert.AreEqual(ONE_ROW_RESULT, doc.OuterXml);
        }
    }
}