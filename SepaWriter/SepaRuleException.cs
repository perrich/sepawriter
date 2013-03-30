using System;
using System.Runtime.Serialization;

namespace Perrich.SepaWriter
{
    /// <summary>
    /// The Exception that is thrown when a SEPA field rule is not validated.
    /// For instance unicity or string size checks.
    /// </summary>
    [Serializable]
    public class SepaRuleException : Exception
    {
        public SepaRuleException()
        {
            throw new InvalidOperationException("A Sepa rule exception should always have an explanation !");
        }

        public SepaRuleException(String message)
            : base(message)
        {
        }

        public SepaRuleException(String message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected SepaRuleException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }
    }
}