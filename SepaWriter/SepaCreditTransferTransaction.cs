using System;
using Perrich.SepaWriter.Utils;

namespace Perrich.SepaWriter
{
    /// <summary>
    /// Define a SEPA Credit Transfer Transaction
    /// </summary>
    public class SepaCreditTransferTransaction : ICloneable
    {
        private decimal amount;
        private SepaIbanData creditor;
        private string endToEndId;
        private string remittanceInformation;

        /// <summary>
        ///     ISO 4217 currency code (default is EUR)
        /// </summary>
        public string Currency  { get; set;  }

        /// <summary>
        ///     Payment Identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///     Creditor IBAN data
        /// </summary>
        /// <exception cref="SepaRuleException">If creditor to set is not valid.</exception>
        public SepaIbanData Creditor
        {
            get { return creditor; }
            set
            {
                if (!value.IsValid)
                    throw new SepaRuleException("Creditor IBAN data are invalid.");
                creditor = value;
            }
        }

        /// <summary>
        ///     The Unique Identifier (if not defined, it's defined as "MessageIdentification/PositionInTransactionsList" by the SepaCreditTransfert)
        /// </summary>
        /// <exception cref="SepaRuleException">If id is null or greatear than 30.</exception>
        public string EndToEndId
        {
            get { return endToEndId; }
            set
            {
                if (value == null || value.Length > 30)
                    throw new SepaRuleException("EndToEndId Length cannot be greater than 30.");

                endToEndId = value;
            }
        }

        /// <summary>
        ///     Remittance information (free comment)
        /// </summary>
        public string RemittanceInformation
        {
            get { return remittanceInformation; }
            set
            {
                remittanceInformation = StringUtils.GetLimitedString(value, 140);
            }
        }

        /// <summary>
        ///     Transfer amount
        /// </summary>
        /// <exception cref="SepaRuleException">If amount has more than two decimals and is lesser than 0.01.</exception>
        public decimal Amount
        {
            get { return amount; }
            set
            {
                if (value < new decimal(0.01) || value > new decimal(999999999.99))
                    throw new SepaRuleException("Invalid amount value: " + value);

                if (Math.Round(value, 2) != value)
                    throw new SepaRuleException("Amount should have at most 2 decimals");

                amount = value;
            }
        }

        /// <summary>
        /// Create a SEPA Credit transfer transaction
        /// </summary>
        public SepaCreditTransferTransaction()
        {
            Currency = Constant.EuroCurrency;
        }

        /// <summary>
        /// Return a copy of this object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var row = (SepaCreditTransferTransaction) MemberwiseClone();
            row.Creditor = (SepaIbanData) Creditor.Clone();

            return row;
        }
    }
}