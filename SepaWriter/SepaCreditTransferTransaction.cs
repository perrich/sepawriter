using System;

namespace SepaWriter
{
    public class SepaCreditTransferTransaction : ICloneable
    {
        /// <summary>
        ///     Creditor IBAN data
        /// </summary>
        public SepaIbanData Creditor;

        /// <summary>
        ///     ISO currency code (default is EUR)
        /// </summary>
        public string Currency = "EUR";

        /// <summary>
        ///     Payment ID
        /// </summary>
        public string Id;

        private decimal _amount;
        private string _endToEndId;
        private string _remittanceInformation;

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
        ///     Remittance information
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
            var row = (SepaCreditTransferTransaction)MemberwiseClone();
            row.Creditor = (SepaIbanData)Creditor.Clone();

            return row;
        }
    }
}