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
    public class SepaCreditTransfer
    {
        private const string EuroCurrency = "EUR";

        /// <summary>
        ///     Purpose of the transaction(s)
        /// </summary>
        public string CategoryPurposeCode;

        /// <summary>
        ///     Creation Date (default is object creation date)
        /// </summary>
        public DateTime CreationDate;

        /// <summary>
        ///     Debtor account ISO currency code (default is EUR)
        /// </summary>
        public string DebtorAccountCurrency = EuroCurrency;

        protected decimal HeaderControlSum = 0;
        public string InitiatingPartyId;
        public string InitiatingPartyName;

        /// <summary>
        ///     Local service instrument code
        /// </summary>
        protected string LocalInstrumentCode;

        /// <summary>
        ///     The Message identifier
        /// </summary>
        public string MessageIdentification;

        /// <summary>
        ///     Number of payment transactions.
        /// </summary>
        protected int NumberOfTransactions = 0;

        protected decimal PaymentControlSum = 0;

        /// <summary>
        ///     The single Payment information identifier (uses Message identifier if not defined)
        /// </summary>
        public string PaymentInfoId;

        /// <summary>
        ///     Payment method
        /// </summary>
        protected string PaymentMethod = "TRF";

        /// <summary>
        ///     Requested Execution Date (default is object creation date)
        /// </summary>
        public DateTime RequestedExecutionDate;

        protected List<SepaCreditTransferTransaction> Transactions = new List<SepaCreditTransferTransaction>();
        private SepaIbanData _debtor;

        public SepaCreditTransfer()
        {
            CreationDate = DateTime.Now;
            RequestedExecutionDate = CreationDate.Date;
        }

        /// <summary>
        ///     Debtor IBAN data
        /// </summary>
        public SepaIbanData Debtor
        {
            get { return _debtor; }
            set
            {
                if (!value.IsValid())
                    throw new SepaRuleException("Debtor IBAN data are invalid.");
                _debtor = value;
            }
        }

        /// <summary>
        ///     Return the XML string
        /// </summary>
        /// <returns></returns>
        public string AsXmlString()
        {
            return GenerateXml().OuterXml;
        }

        /// <summary>
        ///     Save in an XML file
        /// </summary>
        public void Save(string filename)
        {
            GenerateXml().Save(filename);
        }

        /// <summary>
        ///     Get the header control sum in cents.
        /// </summary>
        /// <returns></returns>
        public decimal GetHeaderControlSumInCents()
        {
            return HeaderControlSum*100;
        }

        /// <summary>
        ///     Get the payment control sum in cents.
        /// </summary>
        /// <returns></returns>
        public decimal GetPaymentControlSumInCents()
        {
            return PaymentControlSum*100;
        }

        /// <summary>
        ///     Add an existing credit transfer transaction
        /// </summary>
        /// <param name="transfer"></param>
        public void AddCreditTransfer(SepaCreditTransferTransaction transfer)
        {
            if (transfer == null)
                throw new ArgumentNullException("transfer");

            transfer = (SepaCreditTransferTransaction) transfer.Clone();
            if (transfer.EndToEndId == null)
                transfer.EndToEndId = (PaymentInfoId ?? MessageIdentification) + "/" + (NumberOfTransactions + 1);
            CheckTransactionIdUnicity(transfer.Id, transfer.EndToEndId);
            Transactions.Add(transfer);
            NumberOfTransactions++;
            HeaderControlSum += transfer.Amount;
            PaymentControlSum += transfer.Amount;
        }

        /// <summary>
        ///     Check If the id is not defined in others transactions excepts null values
        /// </summary>
        /// <param name="id"></param>
        /// <param name="endToEndId"></param>
        private void CheckTransactionIdUnicity(string id, string endToEndId)
        {
            if (id == null)
                return;

            if (Transactions.Exists(transfert => transfert.Id != null && transfert.Id == id))
            {
                throw new SepaRuleException("Transaction Id '" + id + "' must be unique in a transfer.");
            }

            if (Transactions.Exists(transfert => transfert.EndToEndId != null && transfert.EndToEndId == endToEndId))
            {
                throw new SepaRuleException("End to End Id '" + endToEndId + "' must be unique in a transfer.");
            }
        }

        /// <summary>
        ///     Is Mandatory data are set ? In other case a SepaRuleException will be thrown
        /// </summary>
        private void CheckMandatoryData()
        {
            if (Transactions.Count == 0)
            {
                throw new SepaRuleException("At least one transaction is needed in a transfer.");
            }
            if (Debtor == null)
            {
                throw new SepaRuleException("The debtor is mandatory.");
            }
            if (string.IsNullOrEmpty(MessageIdentification))
            {
                throw new SepaRuleException("The message identification is mandatory.");
            }
            if (string.IsNullOrEmpty(InitiatingPartyName))
            {
                throw new SepaRuleException("The initial party name is mandatory.");
            }
        }

        /// <summary>
        ///     Generate the XML structure
        /// </summary>
        /// <returns></returns>
        protected XmlDocument GenerateXml()
        {
            CheckMandatoryData();

            var xml = new XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", Encoding.UTF8.BodyName, "yes"));
            var el = (XmlElement) xml.AppendChild(xml.CreateElement("Document"));
            el.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            el.SetAttribute("xmlns", "urn:iso:std:iso:20022:tech:xsd:pain.001.001.03");
            el.NewElement("CstmrCdtTrfInitn");

            // Part 1: Group Header
            XmlElement grpHdr = XmlUtils.GetFirstElement(xml, "CstmrCdtTrfInitn").NewElement("GrpHdr");
            grpHdr.NewElement("MsgId", MessageIdentification);
            grpHdr.NewElement("CreDtTm", String.Format("{0:yyyy-MM-dd\\THH:mm:ss}", CreationDate));
            grpHdr.NewElement("NbOfTxs", NumberOfTransactions);
            grpHdr.NewElement("CtrlSum", XmlUtils.FormatAmount(HeaderControlSum));
            grpHdr.NewElement("InitgPty").NewElement("Nm", InitiatingPartyName);
            if (InitiatingPartyId != null)
                grpHdr.NewElement("InitgPty").NewElement("Id", InitiatingPartyId);

            // Part 2: Payment Information
            XmlElement pmtInf = XmlUtils.GetFirstElement(xml, "CstmrCdtTrfInitn").NewElement("PmtInf");
            pmtInf.NewElement("PmtInfId", PaymentInfoId ?? MessageIdentification);
            if (CategoryPurposeCode != null)
                pmtInf.NewElement("CtgyPurp").NewElement("Cd", CategoryPurposeCode);

            pmtInf.NewElement("PmtMtd", PaymentMethod);
            pmtInf.NewElement("NbOfTxs", NumberOfTransactions);
            pmtInf.NewElement("CtrlSum", XmlUtils.FormatAmount(PaymentControlSum));
            pmtInf.NewElement("PmtTpInf").NewElement("SvcLvl").NewElement("Cd", "SEPA");
            if (LocalInstrumentCode != null)
                XmlUtils.GetFirstElement(xml, "PmtTpInf").NewElement("LclInstr")
                        .NewElement("Cd", LocalInstrumentCode);

            pmtInf.NewElement("ReqdExctnDt", String.Format("{0:yyyy-MM-dd}", RequestedExecutionDate));
            pmtInf.NewElement("Dbtr").NewElement("Nm", Debtor.Name);

            XmlElement dbtrAcct = pmtInf.NewElement("DbtrAcct");
            dbtrAcct.NewElement("Id").NewElement("IBAN", Debtor.Iban);
            dbtrAcct.NewElement("Ccy", DebtorAccountCurrency);

            pmtInf.NewElement("DbtrAgt").NewElement("FinInstnId").NewElement("BIC", Debtor.Bic);
            pmtInf.NewElement("ChrgBr", "SLEV");

            // Part 3: Credit Transfer Transaction Information
            foreach (SepaCreditTransferTransaction transfer in Transactions)
            {
                XmlElement cdtTrfTxInf = pmtInf.NewElement("CdtTrfTxInf");
                XmlElement pmtId = cdtTrfTxInf.NewElement("PmtId");
                if (transfer.Id != null)
                    pmtId.NewElement("InstrId", transfer.Id);
                pmtId.NewElement("EndToEndId", transfer.EndToEndId);
                cdtTrfTxInf.NewElement("Amt")
                           .NewElement("InstdAmt", XmlUtils.FormatAmount(transfer.Amount))
                           .SetAttribute("Ccy", transfer.Currency);
                cdtTrfTxInf.NewElement("CdtrAgt").NewElement("FinInstnId").NewElement("BIC", transfer.Creditor.Bic);
                cdtTrfTxInf.NewElement("Cdtr").NewElement("Nm", transfer.Creditor.Name);
                cdtTrfTxInf.NewElement("CdtrAcct").NewElement("Id").NewElement("IBAN", transfer.Creditor.Iban);
                cdtTrfTxInf.NewElement("RmtInf").NewElement("Ustrd", transfer.RemittanceInformation);
            }

            return xml;
        }
    }
}