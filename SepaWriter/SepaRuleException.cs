using System;

namespace Perrich.SepaWriter
{
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