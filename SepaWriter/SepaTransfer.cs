using System;
using System.Collections.Generic;
using System.Xml;
using SepaWriter.Utils;

namespace SepaWriter
{
    /// <summary>
    ///     Manage SEPA (Single Euro Payments Area) CreditTransfer for SEPA or international order.
    ///     Only one PaymentInformation is managed but it can manage multiple transactions.
    /// </summary>
    public abstract class SepaTransfer<T> where T : SepaTransferTransaction
    {
        protected decimal headerControlSum;
        protected decimal paymentControlSum;
        protected SepaIbanData SepaIban;
        protected readonly List<T> transactions = new List<T>();

        protected SepaSchema schema;

        /// <summary>
        ///     Number of payment transactions.
        /// </summary>
        protected int numberOfTransactions;

        /// <summary>
        ///     Purpose of the transaction(s)
        /// </summary>
        public string CategoryPurposeCode { get; set; }

        /// <summary>
        ///     Creation Date (default is object creation date)
        /// </summary>
        public DateTime CreationDate { get; set; }

        public string InitiatingPartyId { get; set; }

        public string InitiatingPartyName { get; set; }

        /// <summary>
        ///     Local service instrument code
        /// </summary>
        public string LocalInstrumentCode { get; set; }

        /// <summary>
        ///     The Message identifier
        /// </summary>
        public string MessageIdentification { get; set; }

        /// <summary>
        ///     The single Payment information identifier (uses Message identifier if not defined)
        /// </summary>
        public string PaymentInfoId { get; set; }

        /// <summary>
        ///     Requested Execution Date (default is object creation date)
        /// </summary>
        public DateTime RequestedExecutionDate { get; set; }

        /// <summary>
        ///     Get the XML Schema used to create the file
        /// </summary>
        public SepaSchema Schema
        {
            get { return schema; }
            set {
                if (!CheckSchema(value))
                    throw new ArgumentException(schema + " schema is not allowed!");
                schema = value;
            }
        }

        protected SepaTransfer()
        {
            CreationDate = DateTime.Now;
            RequestedExecutionDate = CreationDate.Date;
        }

        /// <summary>
        ///     Header control sum in cents.
        /// </summary>
        /// <returns></returns>
        public decimal HeaderControlSumInCents
        {
            get { return headerControlSum * 100; }
        }

        /// <summary>
        ///    Payment control sum in cents.
        /// </summary>
        /// <returns></returns>
        public decimal PaymentControlSumInCents
        {
            get { return paymentControlSum * 100; }
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
        ///     Add an existing transfer transaction
        /// </summary>
        /// <param name="transfer"></param>
        /// <exception cref="ArgumentNullException">If transfert is null.</exception>
        protected void AddTransfer(T transfer)
        {
            if (transfer == null)
                throw new ArgumentNullException("transfer");

            transfer = (T)transfer.Clone();
            if (transfer.EndToEndId == null)
                transfer.EndToEndId = (PaymentInfoId ?? MessageIdentification) + "/" + (numberOfTransactions + 1);
            CheckTransactionIdUnicity(transfer.Id, transfer.EndToEndId);
            transactions.Add(transfer);
            numberOfTransactions++;
            headerControlSum += transfer.Amount;
            paymentControlSum += transfer.Amount;
        }

        /// <summary>
        ///     Check If the id is not defined in others transactions excepts null values
        /// </summary>
        /// <param name="id"></param>
        /// <param name="endToEndId"></param>
        /// <exception cref="SepaRuleException">If an id is already used.</exception>
        private void CheckTransactionIdUnicity(string id, string endToEndId)
        {
            if (id == null)
                return;

            if (transactions.Exists(transfert => transfert.Id != null && transfert.Id == id))
            {
                throw new SepaRuleException("Transaction Id '" + id + "' must be unique in a transfer.");
            }

            if (transactions.Exists(transfert => transfert.EndToEndId != null && transfert.EndToEndId == endToEndId))
            {
                throw new SepaRuleException("End to End Id '" + endToEndId + "' must be unique in a transfer.");
            }
        }
        
        protected void AddPostalAddressElements(XmlElement pmtInf, string existingElementKey, SepaPostalAddress address)
        {
            var pstlAdr = XmlUtils.GetFirstElement(pmtInf, existingElementKey).NewElement("PstlAdr");
            if (address.AddressType.HasValue)
                pstlAdr.NewElement("AdrTp", address.AddressType.ToString());
            if (!String.IsNullOrEmpty(address.Dept))
                pstlAdr.NewElement("Dept", address.Dept);
            if (!String.IsNullOrEmpty(address.SubDept))
                pstlAdr.NewElement("SubDept", address.SubDept);
            if (!String.IsNullOrEmpty(address.StrtNm))
                pstlAdr.NewElement("StrtNm", address.StrtNm);
            if (!String.IsNullOrEmpty(address.BldgNb))
                pstlAdr.NewElement("BldgNb", address.BldgNb);
            if (!String.IsNullOrEmpty(address.PstCd))
                pstlAdr.NewElement("PstCd", address.PstCd);
            if (!String.IsNullOrEmpty(address.TwnNm))
                pstlAdr.NewElement("TwnNm", address.TwnNm);
            if (!String.IsNullOrEmpty(address.CtrySubDvsn))
                pstlAdr.NewElement("CtrySubDvsn", address.CtrySubDvsn);
            if (!String.IsNullOrEmpty(address.Ctry))
                pstlAdr.NewElement("Ctry", address.Ctry);
            if (address.AdrLine != null)
                foreach (var line in address.AdrLine)
                    pstlAdr.NewElement("AdrLine", line);
        }

        /// <summary>
        ///     Is Mandatory data are set ? In other case a SepaRuleException will be thrown
        /// </summary>
        /// <exception cref="SepaRuleException">If mandatory data is missing.</exception>
        protected virtual void CheckMandatoryData()
        {
            if (transactions.Count == 0)
            {
                throw new SepaRuleException("At least one transaction is needed in a transfer.");
            }
            if (string.IsNullOrEmpty(MessageIdentification))
            {
                throw new SepaRuleException("The message identification is mandatory.");
            }
        }

        /// <summary>
        ///     Generate the XML structure
        /// </summary>
        /// <returns></returns>
        protected abstract XmlDocument GenerateXml();

        protected abstract bool CheckSchema(SepaSchema aSchema);
    }
}