using System;
using System.Collections.Generic;
using System.Globalization;
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
        /// <summary>
        ///     Purpose of the transaction(s)
        /// </summary>
        public string CategoryPurposeCode;

        /// <summary>
        ///     Creation Date (default is object creation date)
        /// </summary>
        public DateTime CreationDate;

        /// <summary>
        ///     Debitor IBAN data
        /// </summary>
        public SepaIbanData Debitor;

        /// <summary>
        ///     Debitor account ISO currency code (default is EUR)
        /// </summary>
        public string DebitorAccountCurrency = "EUR";

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
        ///     The single Payment information identifier
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

        public SepaCreditTransfer()
        {
            CreationDate = DateTime.Now;
            RequestedExecutionDate = CreationDate.Date;
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
        ///     Add a credit transfer transaction in euro
        /// </summary>
        /// <param name="id">The transaction unique identifier</param>
        /// <param name="creditor">The creditor IBAN data</param>
        /// <param name="amount">The amount</param>
        /// <param name="remittanceInformation">The transaction comment</param>
        public void AddCreditTransfer(string id, SepaIbanData creditor, decimal amount,
                                      string remittanceInformation)
        {
            AddCreditTransfer(id, creditor, amount, "EUR", remittanceInformation);
        }

        /// <summary>
        ///     Add a credit transfer transaction
        /// </summary>
        /// <param name="id">The transaction unique identifier</param>
        /// <param name="creditor">The creditor IBAN data</param>
        /// <param name="amount">The amount</param>
        /// <param name="currency">The currency in ISO 4217</param>
        /// <param name="remittanceInformation">The transaction comment</param>
        public void AddCreditTransfer(string id, SepaIbanData creditor, decimal amount, string currency,
                                      string remittanceInformation)
        {
            var transfer = new SepaCreditTransferTransaction
                {
                    Id = id,
                    Currency = currency,
                    Amount = amount,
                    Creditor = creditor,
                    RemittanceInformation = remittanceInformation
                };
            AddCreditTransfer(transfer);
        }

        /// <summary>
        ///     Add an existing credit transfer transaction
        /// </summary>
        /// <param name="transfer"></param>
        public void AddCreditTransfer(SepaCreditTransferTransaction transfer)
        {
            if (transfer == null)
                throw new ArgumentNullException("transfer");

            CheckTransactionIdUnicity(transfer.Id);
            transfer = (SepaCreditTransferTransaction) transfer.Clone();
            transfer.EndToEndId = MessageIdentification + "/" + NumberOfTransactions;
            Transactions.Add(transfer);
            NumberOfTransactions++;
            HeaderControlSum += transfer.Amount;
            PaymentControlSum += transfer.Amount;
        }

        /// <summary>
        /// Check If the id is not defined in others transactions excepts null values
        /// </summary>
        /// <param name="id"></param>
        private void CheckTransactionIdUnicity(string id)
        {
            if (id == null)
                return;

            if (Transactions.Exists(transfert => transfert.Id != null && transfert.Id == id))
            {
                throw new SepaRuleException("Transaction Id '" + id + "' must be unique in a transfer.");
            }
        }

        /// <summary>
        ///     Generate the XML structure
        /// </summary>
        /// <returns></returns>
        protected XmlDocument GenerateXml()
        {
            var xml = new XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", Encoding.UTF8.BodyName, "yes"));
            var el = (XmlElement) xml.AppendChild(xml.CreateElement("Document"));
            el.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            el.SetAttribute("xmlns", "urn:iso:std:iso:20022:tech:xsd:pain.001.001.03");
            el.NewElement("CstmrCdtTrfInitn");

            // -- 1: Group Header
            XmlElement grpHdr = ((XmlElement) xml.SelectSingleNode("//CstmrCdtTrfInitn")).NewElement("GrpHdr");
            grpHdr.NewElement("MsgId", MessageIdentification);
            grpHdr.NewElement("CreDtTm", String.Format("{0:yyyy-MM-dd\\THH:mm:ss}", CreationDate));
            grpHdr.NewElement("NbOfTxs", NumberOfTransactions);
            grpHdr.NewElement("CtrlSum", FormatAmount(HeaderControlSum));
            grpHdr.NewElement("InitgPty").NewElement("Nm", InitiatingPartyName);
            if (InitiatingPartyId != null)
                grpHdr.NewElement("InitgPty").NewElement("Id", InitiatingPartyId);

            // -- 2: Payment Information
            XmlElement pmtInf = ((XmlElement) xml.SelectSingleNode("//CstmrCdtTrfInitn")).NewElement("PmtInf");
            pmtInf.NewElement("PmtInfId", PaymentInfoId);
            if (CategoryPurposeCode != null)
                pmtInf.NewElement("CtgyPurp").NewElement("Cd", CategoryPurposeCode);

            pmtInf.NewElement("PmtMtd", PaymentMethod);
            pmtInf.NewElement("NbOfTxs", NumberOfTransactions);
            pmtInf.NewElement("CtrlSum", FormatAmount(PaymentControlSum));
            pmtInf.NewElement("PmtTpInf").NewElement("SvcLvl").NewElement("Cd", "SEPA");
            if (LocalInstrumentCode != null)
                ((XmlElement) xml.SelectSingleNode("//PmtTpInf")).NewElement("LclInstr")
                                                                 .NewElement("Cd", LocalInstrumentCode);

            pmtInf.NewElement("ReqdExctnDt", String.Format("{0:yyyy-MM-dd}", RequestedExecutionDate));
            pmtInf.NewElement("Dbtr").NewElement("Nm", Debitor.Name);

            XmlElement dbtrAcct = pmtInf.NewElement("DbtrAcct");
            dbtrAcct.NewElement("Id").NewElement("IBAN", Debitor.Iban);
            dbtrAcct.NewElement("Ccy", DebitorAccountCurrency);

            pmtInf.NewElement("DbtrAgt").NewElement("FinInstnId").NewElement("BIC", Debitor.Bic);
            pmtInf.NewElement("ChrgBr", "SLEV");

            // -- 3: Credit Transfer Transaction Information
            foreach (SepaCreditTransferTransaction transfer in Transactions)
            {
                XmlElement cdtTrfTxInf = pmtInf.NewElement("CdtTrfTxInf");
                XmlElement pmtId = cdtTrfTxInf.NewElement("PmtId");
                pmtId.NewElement("InstrId", transfer.Id);
                pmtId.NewElement("EndToEndId", transfer.EndToEndId);
                cdtTrfTxInf.NewElement("Amt")
                           .NewElement("InstdAmt", FormatAmount(transfer.Amount))
                           .SetAttribute("Ccy", transfer.Currency);
                cdtTrfTxInf.NewElement("CdtrAgt").NewElement("FinInstnId").NewElement("BIC", transfer.Creditor.Bic);
                cdtTrfTxInf.NewElement("Cdtr").NewElement("Nm", transfer.Creditor.Name);
                cdtTrfTxInf.NewElement("CdtrAcct").NewElement("Id").NewElement("IBAN", transfer.Creditor.Iban);
                cdtTrfTxInf.NewElement("RmtInf").NewElement("Ustrd", transfer.RemittanceInformation);
            }

            return xml;
        }

        /// <summary>
        ///     Format an amount in requested string format
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        private string FormatAmount(decimal amount)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0:0.##}", amount);
        }
    }
}