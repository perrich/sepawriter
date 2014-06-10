using System;
using System.Text.RegularExpressions;
using Perrich.SepaWriter.Utils;

namespace Perrich.SepaWriter
{
    /// <summary>
    /// Get Creditor or Debtor data (Name and BIC + IBAN)
    /// </summary>
    public class SepaIbanData : ICloneable
    {
        private string bic;
        private string iban;
        private string name;

        // Regex to find space
        private static readonly Regex SpaceRegex = new Regex("\\s+", RegexOptions.Compiled); 

        /// <summary>
        /// The Name of the owner
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                name = StringUtils.GetLimitedString(value, 70);
            }
        }

        /// <summary>
        /// The BIC Code
        /// </summary>
        /// <exception cref="SepaRuleException">If BIC hasn't 8 or 11 characters.</exception>
        public string Bic
        {
            get { return bic; }
            set
            {
                if (value == null || (value.Length != 8 && value.Length != 11))
                    throw new SepaRuleException("Invalid BIC code.");
                bic = value;
            }
        }

        /// <summary>
        /// The IBAN Number
        /// </summary>
        /// <exception cref="SepaRuleException">If IBAN length is not between 14 and 34 characters.</exception>
        public string Iban
        {
            get { return iban; }
            set
            {
                if (value == null || value.Length < 14 || value.Length > 34)
                    throw new SepaRuleException("Invalid IBAN code.");
                iban = SpaceRegex.Replace(value, string.Empty);
            }
        }

        /// <summary>
        ///     Is data is well set to be used
        /// </summary>
        /// <returns></returns>
        public bool IsValid
        {
            get { return !string.IsNullOrEmpty(bic) && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(iban); }
        }

        /// <summary>
        /// Return a copy of this object
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}