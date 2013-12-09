using System;
using Perrich.SepaWriter.Utils;

namespace Perrich.SepaWriter
{
    /// <summary>
    ///     Base of a SEPA Transfer Transaction
    /// </summary>
    public abstract class SepaTransferTransaction : ICloneable
    {
        private decimal amount;
        protected SepaIbanData SepaIban;
        private string endToEndId;
        private string remittanceInformation;

        /// <summary>
        ///     Create a SEPA Credit transfer transaction
        /// </summary>
        protected SepaTransferTransaction()
        {
            Currency = Constant.EuroCurrency;
        }

        /// <summary>
        ///     ISO 4217 currency code (default is EUR)
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        ///     Payment Identifier
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        ///     The Unique Identifier (if not defined, it's defined as "MessageIdentification/PositionInTransactionsList" by the
        ///     SepaCreditTransfert)
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
            set { remittanceInformation = StringUtils.GetLimitedString(value, 140); }
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
        ///     Return a copy of this object
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            var row = (SepaTransferTransaction)MemberwiseClone();
            row.SepaIban = (SepaIbanData)SepaIban.Clone();

            return row;
        }
    }
}