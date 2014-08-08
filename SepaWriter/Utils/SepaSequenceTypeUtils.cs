using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perrich.SepaWriter.Utils
{
    public static class SepaSequenceTypeUtils
    {
        /// <summary>
        ///     Get awaited XML string from Enum value
        /// </summary>
        /// <param name="seqTp">Enum value</param>
        /// <returns>Awaited XML string</returns>
        public static string SepaSequenceTypeToString(SepaSequenceType seqTp)
        {
            switch (seqTp)
            {
                case SepaSequenceType.OOFF:
                    return "OOFF";
                case SepaSequenceType.FIRST:
                    return "FRST";
                case SepaSequenceType.RCUR:
                    return "RCUR";
                case SepaSequenceType.FINAL:
                    return "FNAL";
                default:
                    throw new ArgumentException("Unknown Sequence Type : " + seqTp);
            }
        }

        /// <summary>
        ///     Get the Enum value from XML valid value for SeqTp
        /// </summary>
        /// <param name="seqTp">XML valid value for SeqTp</param>
        /// <returns>Enum value from SepaSequenceType</returns>
        public static SepaSequenceType SepaSequenceTypeFromString(string seqTp)
        {
            switch (seqTp)
            {
                case "OOFF":
                    return SepaSequenceType.OOFF;
                case "FRST":
                    return SepaSequenceType.FIRST;
                case "RCUR":
                    return SepaSequenceType.RCUR;
                case "FNAL":
                    return SepaSequenceType.FINAL;
                default:
                    throw new ArgumentException("Unknown Sequence Type : " + seqTp);
            }
        }
    }
}
