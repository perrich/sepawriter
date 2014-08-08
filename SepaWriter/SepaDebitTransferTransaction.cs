using System;

namespace Perrich.SepaWriter
{
    /// <summary>
    ///     Define a SEPA Credit Transfer Transaction
    /// </summary>
    public class SepaDebitTransferTransaction : SepaTransferTransaction
    {
        /// <summary>
        ///     Date on which the direct debit mandate has been signed by the debtor.
        /// </summary>
        public DateTime DateOfSignature { get; set; }

        /// <summary>
        ///     Unique identification, as assigned by the debtor, to unambiguously identify the mandate.
        /// </summary>
        public string MandateIdentification { get; set; }

        /// <summary>
        ///     Sequence Type (default is "OOFF")
        /// </summary>
        public SepaSequenceType SequenceType { get; set; }

        /// <summary>
        ///     Debtor IBAN data
        /// </summary>
        /// <exception cref="SepaRuleException">If debtor to set is not valid.</exception>
        public SepaIbanData Debtor
        {
            get { return SepaIban; }
            set
            {
                if (!value.IsValid)
                    throw new SepaRuleException("Debtor IBAN data are invalid.");
                SepaIban = value;
                
            }
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public SepaDebitTransferTransaction()
        {
            SequenceType = SepaSequenceType.OOFF;
        }
    }
}