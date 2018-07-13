using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Perrich.SepaWriter.Utils;

namespace Perrich.SepaWriter
{
    /// <summary>
    ///     Manage SEPA (Single Euro Payments Area) CreditTransfer for SEPA or international order.
    ///     Only one PaymentInformation is managed but it can manage multiple transactions.
    /// </summary>
    public class SepaDebitTransfer : SepaTransfer<SepaDebitTransferTransaction>
    {
        /// <summary>
        ///     creditor account ISO currency code (default is EUR)
        /// </summary>
        public string CreditorAccountCurrency { get; set; }

        /// <summary>
        ///     Unique and unambiguous identification of a person. SEPA creditor
        /// </summary>
        public string PersonId { get; set; }

        /// <summary>
        /// Create a Sepa Debit Transfer using Pain.008.001.02 schema
        /// </summary>
        public SepaDebitTransfer()
        {
            CreditorAccountCurrency = Constant.EuroCurrency;
            LocalInstrument = new LocalInstrument() { Code = "CORE" };
            schema = SepaSchema.Pain00800102;
        }

        /// <summary>
        ///     Creditor IBAN data
        /// </summary>
        /// <exception cref="SepaRuleException">If creditor to set is not valid.</exception>
        public SepaIbanData Creditor
        {
            get { return SepaIban; }
            set
            {
                if (!value.IsValid || value.UnknownBic)
                    throw new SepaRuleException("Creditor IBAN data are invalid.");
                SepaIban = value;
            }
        }

        /// <summary>
        ///     Is Mandatory data are set ? In other case a SepaRuleException will be thrown
        /// </summary>
        /// <exception cref="SepaRuleException">If mandatory data is missing.</exception>
        protected override void CheckMandatoryData()
        {
            base.CheckMandatoryData();

            if (Creditor == null)
            {
                throw new SepaRuleException("The creditor is mandatory.");
            }
        }

        /// <summary>
        ///     Add an existing transfer transaction
        /// </summary>
        /// <param name="transfer"></param>
        /// <exception cref="ArgumentNullException">If transfert is null.</exception>
        public void AddDebitTransfer(SepaDebitTransferTransaction transfer)
        {
            AddTransfer(transfer);
        }

        /// <summary>
        ///     Generate the XML structure
        /// </summary>
        /// <returns></returns>
        protected override XmlDocument GenerateXml()
        {
            CheckMandatoryData();

            var xml = new XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", Encoding.UTF8.BodyName, "yes"));
            var el = (XmlElement)xml.AppendChild(xml.CreateElement("Document"));
            el.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            el.SetAttribute("xmlns", "urn:iso:std:iso:20022:tech:xsd:" + SepaSchemaUtils.SepaSchemaToString(schema));
            el.NewElement("CstmrDrctDbtInitn");

            // Part 1: Group Header
            var grpHdr = XmlUtils.GetFirstElement(xml, "CstmrDrctDbtInitn").NewElement("GrpHdr");
            grpHdr.NewElement("MsgId", MessageIdentification);
            grpHdr.NewElement("CreDtTm", StringUtils.FormatDateTime(CreationDate));
            grpHdr.NewElement("NbOfTxs", numberOfTransactions);
            grpHdr.NewElement("CtrlSum", StringUtils.FormatAmount(headerControlSum));

            if (InitiatingParty != null)
            {
                grpHdr.NewElement("InitgPty");

                if (!string.IsNullOrWhiteSpace(InitiatingParty.Name))
                {
                    XmlUtils.GetFirstElement(grpHdr, "InitgPty").
                        NewElement("Nm", InitiatingParty.Name);
                }

                if (InitiatingParty.Identification != null)
                {
                    var othr = XmlUtils.GetFirstElement(grpHdr, "InitgPty").
                                            NewElement("Id").NewElement("OrgId").
                                            NewElement("Othr");

                    othr.NewElement("Id", InitiatingParty.Identification.Id);
                    othr.NewElement("Issr", InitiatingParty.Identification.Issuer);
                }
            }

            // Part 2: Payment Information for each Sequence Type.
            foreach (SepaSequenceType seqTp in Enum.GetValues(typeof(SepaSequenceType)))
            {
                var seqTransactions = transactions.FindAll(d => d.SequenceType == seqTp);
                var pmtInf = GeneratePaymentInformation(xml, seqTp, seqTransactions);
                // If a payment information has been created
                if (pmtInf != null)
                {
                    // Part 3: Debit Transfer Transaction Information
                    foreach (var transfer in seqTransactions)
                    {
                        GenerateTransaction(pmtInf, transfer);
                    }
                }
            }

            return xml;
        }

        /// <summary>
        /// Generate a Payment Information node for a Sequence Type.
        /// </summary>
        /// <param name="xml">The XML object to write</param>
        /// <param name="sqType">The Sequence Type</param>
        /// <param name="seqTransactions">The transactions of the specified type</param>
        private XmlElement GeneratePaymentInformation(XmlDocument xml, SepaSequenceType sqType, IEnumerable<SepaDebitTransferTransaction> seqTransactions)
        {
            int controlNumber = 0;
            decimal controlSum = 0;

            // We check the number of transaction to write and the sum due to the Sequence Type.
            foreach (var transfer in seqTransactions)
            {
                controlNumber += 1;
                controlSum += transfer.Amount;
            }

            // If there is no transaction, we end the method here.
            if (controlNumber == 0)
                return null;

            var pmtInf = XmlUtils.GetFirstElement(xml, "CstmrDrctDbtInitn").NewElement("PmtInf");
            pmtInf.NewElement("PmtInfId", PaymentInfoId ?? MessageIdentification);

            pmtInf.NewElement("PmtMtd", Constant.DebitTransfertPaymentMethod);
            pmtInf.NewElement("NbOfTxs", controlNumber);
            pmtInf.NewElement("CtrlSum", StringUtils.FormatAmount(controlSum));

            var pmtTpInf = pmtInf.NewElement("PmtTpInf");
            pmtTpInf.NewElement("SvcLvl").NewElement("Cd", "SEPA");

            if (LocalInstrument != null)
            {
                var lclInstr = pmtTpInf.NewElement("LclInstr");

                if (!string.IsNullOrWhiteSpace(LocalInstrument.Code))
                    lclInstr.NewElement("Cd", LocalInstrument.Code);

                if (!string.IsNullOrWhiteSpace(LocalInstrument.Proprietary))
                    lclInstr.NewElement("Prtry", LocalInstrument.Proprietary);
            }

            if (CategoryPurpose != null)
            {
                var ctgyPurp = pmtTpInf.NewElement("CtgyPurp");

                if (!string.IsNullOrWhiteSpace(CategoryPurpose.Code))
                    ctgyPurp.NewElement("Cd", CategoryPurpose.Code);

                if (!string.IsNullOrWhiteSpace(CategoryPurpose.Proprietary))
                    ctgyPurp.NewElement("Prtry", CategoryPurpose.Proprietary);
            }

            pmtTpInf.NewElement("LclInstrm").NewElement("Cd", LocalInstrument.Code);
            pmtTpInf.NewElement("SeqTp", SepaSequenceTypeUtils.SepaSequenceTypeToString(sqType));

            pmtInf.NewElement("ReqdColltnDt", StringUtils.FormatDate(RequestedExecutionDate));
            pmtInf.NewElement("Cdtr").NewElement("Nm", Creditor.Name);

            var dbtrAcct = pmtInf.NewElement("CdtrAcct");
            dbtrAcct.NewElement("Id").NewElement("IBAN", Creditor.Iban);
            dbtrAcct.NewElement("Ccy", CreditorAccountCurrency);

            pmtInf.NewElement("CdtrAgt").NewElement("FinInstnId").NewElement("BIC", Creditor.Bic);
            pmtInf.NewElement("ChrgBr", "SLEV");

            var othr = pmtInf.NewElement("CdtrSchmeId").NewElement("Id")
                    .NewElement("PrvtId")
                        .NewElement("Othr");
            othr.NewElement("Id", PersonId);
            othr.NewElement("SchmeNm").NewElement("Prtry", "SEPA");

            return pmtInf;
        }

        /// <summary>
        /// Generate the Transaction XML part
        /// </summary>
        /// <param name="pmtInf">The root nodes for a transaction</param>
        /// <param name="transfer">The transaction to generate</param>
        private static void GenerateTransaction(XmlElement pmtInf, SepaDebitTransferTransaction transfer)
        {
            var cdtTrfTxInf = pmtInf.NewElement("DrctDbtTxInf");
            var pmtId = cdtTrfTxInf.NewElement("PmtId");
            if (transfer.Id != null)
                pmtId.NewElement("InstrId", transfer.Id);
            pmtId.NewElement("EndToEndId", transfer.EndToEndId);
            cdtTrfTxInf.NewElement("InstdAmt", StringUtils.FormatAmount(transfer.Amount)).SetAttribute("Ccy", transfer.Currency);

            var mndtRltdInf = cdtTrfTxInf.NewElement("DrctDbtTx").NewElement("MndtRltdInf");
            mndtRltdInf.NewElement("MndtId", transfer.MandateIdentification);
            mndtRltdInf.NewElement("DtOfSgntr", StringUtils.FormatDate(transfer.DateOfSignature));

            XmlUtils.CreateBic(cdtTrfTxInf.NewElement("DbtrAgt"), transfer.Debtor);
            cdtTrfTxInf.NewElement("Dbtr").NewElement("Nm", transfer.Debtor.Name);
            cdtTrfTxInf.NewElement("DbtrAcct").NewElement("Id").NewElement("IBAN", transfer.Debtor.Iban);

            if (!string.IsNullOrEmpty(transfer.RemittanceInformation))
                cdtTrfTxInf.NewElement("RmtInf").NewElement("Ustrd", transfer.RemittanceInformation);
        }

        protected override bool CheckSchema(SepaSchema aSchema)
        {
            return aSchema == SepaSchema.Pain00800102 || aSchema == SepaSchema.Pain00800103;
        }
    }
}
