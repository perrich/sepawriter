using System;

namespace Perrich.SepaWriter
{
    public class SepaCreditTransferTransaction : ICloneable
    {
        /// <summary>
        ///     ISO 4217 currency code (default is EUR)
        /// </summary>
        public string Currency = "EUR";

        /// <summary>
        ///     Payment Identifier
        /// </summary>
        public string Id;

        private decimal _amount;
        private SepaIbanData _creditor;
        private string _endToEndId;
        private string _remittanceInformation;

        /// <summary>
        ///     Creditor IBAN data
        /// </summary>
        public SepaIbanData Creditor
        {
            get { return _creditor; }
            set
            {
                if (!value.IsValid())
                    throw new SepaRuleException("Creditor IBAN data are invalid.");
                _creditor = value;
            }
        }

        /// <summary>
        ///     The Unique Identifier (if not defined, it's defined as "MessageIdentification/PositionInTransactionsList" by the SepaCreditTransfert)
        /// </summary>
        public string EndToEndId
        {
            get { return _endToEndId; }
            set
            {
                if (value == null || value.Length > 30)
                    throw new SepaRuleException("EndToEndId Length cannot be greater than 30.");

                _endToEndId = value;
            }
        }

        /// <summary>
        ///     Remittance information (free comment)
        /// </summary>
        public string RemittanceInformation
        {
            get { return _remittanceInformation; }
            set
            {
                const int allowedLength = 140;

                if (value != null && value.Length > allowedLength)
                {
                    _remittanceInformation = value.Substring(0, allowedLength);
                }
                else
                {
                    _remittanceInformation = value;
                }
            }
        }

        /// <summary>
        ///     Transfer amount
        /// </summary>
        public decimal Amount
        {
            get { return _amount; }
            set
            {
                if (value < new decimal(0.01) || value > new decimal(999999999.99))
                    throw new SepaRuleException("Invalid amount value: " + value);

                if (Math.Round(value, 2) != value)
                    throw new SepaRuleException("Amount should have at most 2 decimals");

                _amount = value;
            }
        }

        public object Clone()
        {
            var row = (SepaCreditTransferTransaction) MemberwiseClone();
            row.Creditor = (SepaIbanData) Creditor.Clone();

            return row;
        }
    }
}