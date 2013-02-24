using System;

namespace Perrich.SepaWriter
{
    /// <summary>
    ///     Get Creditor or Debtor data (Name and BIC + IBAN)
    /// </summary>
    public class SepaIbanData : ICloneable
    {
        private string _bic;
        private string _iban;
        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                const int allowedLength = 70;

                if (value != null && value.Length > allowedLength)
                {
                    _name = value.Substring(0, allowedLength);
                }
                else
                {
                    _name = value;
                }
            }
        }

        public string Bic
        {
            get { return _bic; }
            set
            {
                if (value == null || (value.Length != 8 && value.Length != 11))
                    throw new SepaRuleException("Invalid BIC code.");
                _bic = value;
            }
        }

        public string Iban
        {
            get { return _iban; }
            set
            {
                if (value == null || value.Length < 14 || value.Length > 34)
                    throw new SepaRuleException("Invalid IBAN code.");
                _iban = value;
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        ///     Is data is well set to be used
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Bic) && !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Iban);
        }
    }
}