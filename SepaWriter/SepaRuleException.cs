using System;

namespace Perrich.SepaWriter
{
    /// <summary>
    /// The Exception that is thrown when a SEPA field rule is not validated.
    /// For instance unicity or string size checks.
    /// </summary>
    public class SepaRuleException : Exception
    {
        public SepaRuleException(String message)
            : base(message)
        {
        }

        public SepaRuleException(String message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}