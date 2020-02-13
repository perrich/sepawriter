using System;

namespace SepaWriter.Utils
{
    public static class SepaInstructionForCreditorUtils
    {
        /// <summary>
        ///     Get awaited XML string from Enum value
        /// </summary>
        /// <param name="seqTp">Enum value</param>
        /// <returns>Awaited XML string</returns>
        public static string SepaInstructionForCreditorToString(SepaInstructionForCreditor.SepaInstructionForCreditorCode seqTp)
        {
            switch (seqTp)
            {
                case SepaInstructionForCreditor.SepaInstructionForCreditorCode.CHQB:
                    return "CHQB";
                case SepaInstructionForCreditor.SepaInstructionForCreditorCode.HOLD:
                    return "HOLD";
                case SepaInstructionForCreditor.SepaInstructionForCreditorCode.PHOB:
                    return "PHOB";
                case SepaInstructionForCreditor.SepaInstructionForCreditorCode.TELB:
                    return "TELB";
                default:
                    throw new ArgumentException("Unknown Charge Bearer : " + seqTp);
            }
        }

        /// <summary>
        ///     Get the Enum value from XML valid value for SeqTp
        /// </summary>
        /// <param name="seqTp">XML valid value for SeqTp</param>
        /// <returns>Enum value from SepaInstructionForCreditor</returns>
        public static SepaInstructionForCreditor.SepaInstructionForCreditorCode SepaInstructionForCreditorFromString(string seqTp)
        {
            switch (seqTp)
            {
                case "CHQB":
                    return SepaInstructionForCreditor.SepaInstructionForCreditorCode.CHQB;
                case "HOLD":
                    return SepaInstructionForCreditor.SepaInstructionForCreditorCode.HOLD;
                case "PHOB":
                    return SepaInstructionForCreditor.SepaInstructionForCreditorCode.PHOB;
                case "TELB":
                    return SepaInstructionForCreditor.SepaInstructionForCreditorCode.TELB;
                default:
                    throw new ArgumentException("Unknown Instruction for Creditor : " + seqTp);
            }
        }
    }
}