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
        private string other;
        private string bic;
        private string iban;
        private string name;
        private SepaPostalAddress address;
        private SepaPostalAddress agentAddress;
        private bool withoutBic;

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

        public SepaPostalAddress Address
        {
            get { return address; }
            set
            {
                if (!value.IsValid)
                    throw new SepaRuleException("Iban Address data is invalid.");
                address = value;
            }
        }

        public SepaPostalAddress AgentAddress
        {
            get { return agentAddress; }
            set
            {
                if (!value.IsValid)
                    throw new SepaRuleException("Iban Address data is invalid.");
                agentAddress = value;
            }
        }

        /// <summary>
        /// Is the BIC code unknown?
        /// </summary>
        public bool UnknownBic
        {
            get { return withoutBic; }
            set { withoutBic = value; }
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
				if (value == null || (value.Length != 8 && value.Length != 11)) {
					throw new SepaRuleException(string.Format("Null or Invalid length of BIC/swift code \"{0}\", must be 8 or 11 chars.", value));
				}
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
                if (value != null && (value.Length < 14 || value.Length > 34))
                    throw new SepaRuleException(string.Format("Null or Invalid length of IBAN code \"{0}\", must contain between 14 and 34 characters.", value));
                iban = SpaceRegex.Replace(value, string.Empty);
            }
        }

        /// <summary>
        /// Represents the Id only. SchmeNm and Issr are not presently accounted for
        /// </summary>
        public string Other
        {
            get { return other; }
            
            set
            {
                if (value != null && value.Length > 34)
                    throw new SepaRuleException(string.Format("Invalid length of Othr > Id \"{0}\", must contain a maximum of 34 characters.", value));
                other = SpaceRegex.Replace(value, string.Empty);
            }
        }

        /// <summary>
        /// Is data is well set to be used
        /// </summary>
        /// <returns></returns>
        public bool IsValid
        {
            get { return (!string.IsNullOrEmpty(bic) || withoutBic) && !string.IsNullOrEmpty(name) && (!string.IsNullOrEmpty(iban) || !string.IsNullOrEmpty(other)); }
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

    public enum PostalAddressType
    {
        ADDR,
        PBOX,
        HOME,
        BIZZ,
        MLTO,
        DLVY
    }
}