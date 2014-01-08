using System;
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
        public string DebtorAccountCurrency { get; set; }

        /// <summary>
        ///     Unique and unambiguous identification of a person. SEPA creditor
        /// </summary>
        public string PersonId { get; set; }

        /// <summary>
        ///     Sequence Type (default is "OOFF")
        /// </summary>
        public string SequenceType { get; set; }

        /// <summary>
        /// Create a Sepa Debit Transfer using Pain.008.001.02 schema
        /// </summary>
        public SepaDebitTransfer()
        {
            DebtorAccountCurrency = Constant.EuroCurrency;
            LocalInstrumentCode = "CORE";
            SequenceType = "OOFF";
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
                if (!value.IsValid)
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
            grpHdr.NewElement("InitgPty").NewElement("Nm", InitiatingPartyName);
            if (InitiatingPartyId != null)
                grpHdr.NewElement("InitgPty").NewElement("Id", InitiatingPartyId);

            // Part 2: Payment Information
            var pmtInf = XmlUtils.GetFirstElement(xml, "CstmrDrctDbtInitn").NewElement("PmtInf");
            pmtInf.NewElement("PmtInfId", PaymentInfoId ?? MessageIdentification);
            if (CategoryPurposeCode != null)
                pmtInf.NewElement("CtgyPurp").NewElement("Cd", CategoryPurposeCode);

            pmtInf.NewElement("PmtMtd", Constant.DebitTransfertPaymentMethod);
            pmtInf.NewElement("NbOfTxs", numberOfTransactions);
            pmtInf.NewElement("CtrlSum", StringUtils.FormatAmount(paymentControlSum));
            pmtInf.NewElement("PmtTpInf").NewElement("SvcLvl").NewElement("Cd", "SEPA");
            XmlUtils.GetFirstElement(xml, "PmtTpInf").NewElement("LclInstrm")
                        .NewElement("Cd", LocalInstrumentCode);
            XmlUtils.GetFirstElement(xml, "PmtTpInf").NewElement("SeqTp", SequenceType);

            pmtInf.NewElement("ReqdColltnDt", StringUtils.FormatDate(RequestedExecutionDate));
            pmtInf.NewElement("Cdtr").NewElement("Nm", Creditor.Name);

            var dbtrAcct = pmtInf.NewElement("CdtrAcct");
            dbtrAcct.NewElement("Id").NewElement("IBAN", Creditor.Iban);
            dbtrAcct.NewElement("Ccy", DebtorAccountCurrency);

            pmtInf.NewElement("CdtrAgt").NewElement("FinInstnId").NewElement("BIC", Creditor.Bic);
            pmtInf.NewElement("ChrgBr", "SLEV");

            var othr = pmtInf.NewElement("CdtrSchmeId").NewElement("Id")
                    .NewElement("PrvtId")
                        .NewElement("Othr");
            othr.NewElement("Id", PersonId);
            othr.NewElement("SchmeNm").NewElement("Prtry", "SEPA");
            // Part 3: Debit Transfer Transaction Information
            foreach (var transfer in transactions)
            {
                GenerateTransaction(pmtInf, transfer);
            }

            return xml;
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

            cdtTrfTxInf.NewElement("DbtrAgt").NewElement("FinInstnId").NewElement("BIC", transfer.Debtor.Bic);
            cdtTrfTxInf.NewElement("Dbtr").NewElement("Nm", transfer.Debtor.Name);
            cdtTrfTxInf.NewElement("DbtrAcct").NewElement("Id").NewElement("IBAN", transfer.Debtor.Iban);
            
            if (!string.IsNullOrEmpty(transfer.RemittanceInformation))
                cdtTrfTxInf.NewElement("RmtInf").NewElement("Ustrd", transfer.RemittanceInformation);
        }

        protected override bool CheckSchema(SepaSchema schema)
        {
            return schema == SepaSchema.Pain00800102 || schema == SepaSchema.Pain00800103;
        }
    }
}