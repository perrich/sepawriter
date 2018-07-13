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
    public class SepaCreditTransfer : SepaTransfer<SepaCreditTransferTransaction>
    {
        /// <summary>
        ///     Debtor account ISO currency code (default is EUR)
        /// </summary>
        public string DebtorAccountCurrency { get; set; }

        /// <summary>
        ///     Is it an international credit transfer?
        /// </summary>
        public bool IsInternational { get; set; }

        /// <summary>
        ///     Batch booking for international credit transfer
        /// </summary>
        public SepaBatchBooking? BatchBooking { get; set; }

        /// <summary>
        ///     Charger bearer for international credit transfer
        /// </summary>
        public SepaChargeBearer? ChargeBearer { get; set; }

        /// <summary>
        /// Create a Sepa Credit Transfer using Pain.001.001.03 schema
        /// </summary>
        public SepaCreditTransfer()
        {
            DebtorAccountCurrency = Constant.EuroCurrency;
            schema = SepaSchema.Pain00100103;
            IsInternational = false;
        }

        /// <summary>
        ///     Debtor IBAN data
        /// </summary>
        /// <exception cref="SepaRuleException">If debtor to set is not valid.</exception>
        public SepaIbanData Debtor
        {
            get { return SepaIban; }
            set
            {
                if (!value.IsValid || value.UnknownBic)
                    throw new SepaRuleException("Debtor IBAN data are invalid.");
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

            if (Debtor == null)
            {
                throw new SepaRuleException("The debtor is mandatory.");
            }
        }

        /// <summary>
        ///     Add an existing transfer transaction
        /// </summary>
        /// <param name="transfer"></param>
        /// <exception cref="ArgumentNullException">If transfert is null.</exception>
        public void AddCreditTransfer(SepaCreditTransferTransaction transfer)
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
            el.NewElement("CstmrCdtTrfInitn");

            // Part 1: Group Header
            var grpHdr = XmlUtils.GetFirstElement(xml, "CstmrCdtTrfInitn").NewElement("GrpHdr");
            grpHdr.NewElement("MsgId", MessageIdentification);
            grpHdr.NewElement("CreDtTm", StringUtils.FormatDateTime(CreationDate));
            grpHdr.NewElement("NbOfTxs", numberOfTransactions);
            grpHdr.NewElement("CtrlSum", StringUtils.FormatAmount(headerControlSum));

            if (InitiatingParty != null)
            {
                var initgPty = grpHdr.NewElement("InitgPty");

                if (!string.IsNullOrWhiteSpace(InitiatingParty.Name))
                {
                    initgPty.NewElement("Nm", InitiatingParty.Name);
                }

                if (InitiatingParty.Identification != null)
                {
                    var othr = initgPty.NewElement("Id").
                                        NewElement("OrgId").
                                        NewElement("Othr");

                    othr.NewElement("Id", InitiatingParty.Identification.Id);
                    othr.NewElement("Issr", InitiatingParty.Identification.Issuer);
                }
            }

            if (ForwardingAgent != null)
            {
                var fwdgAgt = grpHdr.NewElement("FwdgAgt");

                if (ForwardingAgent.FinancialInstitutionId != null)
                {
                    var finInstnId = fwdgAgt.NewElement("FinInstnId");

                    finInstnId.NewElement("BIC", ForwardingAgent.FinancialInstitutionId.BIC);
                    finInstnId.NewElement("Nm", ForwardingAgent.FinancialInstitutionId.Name);
                }

                if (ForwardingAgent.BranchId != null)
                {
                    fwdgAgt.NewElement("BrnchId")
                            .NewElement("Id", ForwardingAgent.BranchId.Id);

                }

            }

            // Part 2: Payment Information
            var pmtInf = XmlUtils.GetFirstElement(xml, "CstmrCdtTrfInitn").NewElement("PmtInf");
            pmtInf.NewElement("PmtInfId", PaymentInfoId ?? MessageIdentification);

            pmtInf.NewElement("PmtMtd", Constant.CreditTransfertPaymentMethod);

            if (BatchBooking.HasValue)
                pmtInf.NewElement("BtchBookg", SepaBatchBookingUtils.SepaBatchBookingToString(BatchBooking.Value));

            pmtInf.NewElement("NbOfTxs", numberOfTransactions);
            pmtInf.NewElement("CtrlSum", StringUtils.FormatAmount(paymentControlSum));

            var pmtTpInf = pmtInf.NewElement("PmtTpInf");

            if (IsInternational)
            {
                pmtTpInf.NewElement("InstrPrty", "NORM");
            }
            else
            {
                pmtTpInf.NewElement("SvcLvl").NewElement("Cd", "SEPA");
            }

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

            pmtInf.NewElement("ReqdExctnDt", StringUtils.FormatDate(RequestedExecutionDate));

            var dbtr = pmtInf.NewElement("Dbtr");

            dbtr.NewElement("Nm", Debtor.Name);

            if (Debtor.Identification != null && !string.IsNullOrWhiteSpace(Debtor.Identification.Id))
            {
                dbtr.NewElement("Id").
                     NewElement("OrgId").
                     NewElement("Othr").
                     NewElement("Id", Debtor.Identification.Id);
            }

            var dbtrAcct = pmtInf.NewElement("DbtrAcct");
            dbtrAcct.NewElement("Id").NewElement("IBAN", Debtor.Iban);
            dbtrAcct.NewElement("Ccy", DebtorAccountCurrency);

            var dbtrAgt = pmtInf.NewElement("DbtrAgt");
            dbtrAgt.NewElement("FinInstnId").NewElement("BIC", Debtor.Bic);

            if (ChargeBearer.HasValue)
                pmtInf.NewElement("ChrgBr", SepaChargeBearerUtils.SepaChargeBearerToString(ChargeBearer.Value));

            // Part 3: Credit Transfer Transaction Information
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
        private void GenerateTransaction(XmlElement pmtInf, SepaCreditTransferTransaction transfer)
        {
            var cdtTrfTxInf = pmtInf.NewElement("CdtTrfTxInf");

            var pmtId = cdtTrfTxInf.NewElement("PmtId");

            if (transfer.Id != null)
                pmtId.NewElement("InstrId", transfer.Id);

            if (transfer.EndToEndId != null)
                pmtId.NewElement("EndToEndId", transfer.EndToEndId);

            cdtTrfTxInf.NewElement("Amt")
                       .NewElement("InstdAmt", StringUtils.FormatAmount(transfer.Amount))
                       .SetAttribute("Ccy", transfer.Currency);

            XmlUtils.CreateBic(cdtTrfTxInf.NewElement("CdtrAgt"), transfer.Creditor);

            var cdtr = cdtTrfTxInf.NewElement("Cdtr");

            if (!string.IsNullOrWhiteSpace(transfer.Creditor.Name))
                cdtr.NewElement("Nm", transfer.Creditor.Name);

            if (transfer.Creditor.Identification != null && !string.IsNullOrWhiteSpace(transfer.Creditor.Identification.Id))
            {
                cdtr.NewElement("Id").
                         NewElement("PrvtId").
                         NewElement("Othr").
                         NewElement("Id", transfer.Creditor.Identification.Id);
            }

            var cdtrAcct = cdtTrfTxInf.NewElement("CdtrAcct");

            cdtrAcct.NewElement("Id").NewElement("IBAN", transfer.Creditor.Iban);

            if (IsInternational && transfer.SepaInstructionForCreditor != null)
            {
                var instr = cdtTrfTxInf.NewElement("InstrForCdtrAgt");
                instr.NewElement("Cd", transfer.SepaInstructionForCreditor.Code);
                if (!string.IsNullOrEmpty(transfer.SepaInstructionForCreditor.Comment))
                {
                    instr.NewElement("InstrInf", transfer.SepaInstructionForCreditor.Comment);
                }
            }

            if (!string.IsNullOrEmpty(transfer.Purpose))
            {
                cdtTrfTxInf.NewElement("Purp").NewElement("Cd", transfer.Purpose);
            }

            if (IsInternational && !string.IsNullOrEmpty(transfer.RegulatoryReportingCode))
            {
                cdtTrfTxInf.NewElement("RgltryRptg").NewElement("Dtls").NewElement("Cd", transfer.RegulatoryReportingCode);
            }

            if (!string.IsNullOrEmpty(transfer.RemittanceInformation))
            {
                cdtTrfTxInf.NewElement("RmtInf").NewElement("Ustrd", transfer.RemittanceInformation);
            }
        }
        protected override bool CheckSchema(SepaSchema aSchema)
        {
            return aSchema == SepaSchema.Pain00100103 || aSchema == SepaSchema.Pain00100104;
        }
    }
}
