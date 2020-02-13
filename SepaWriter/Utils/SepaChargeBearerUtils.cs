using System;

namespace SepaWriter.Utils
{
    public static class SepaChargeBearerUtils
    {
        /// <summary>
        ///     Get awaited XML string from Enum value
        /// </summary>
        /// <param name="seqTp">Enum value</param>
        /// <returns>Awaited XML string</returns>
        public static string SepaChargeBearerToString(SepaChargeBearer seqTp)
        {
            switch (seqTp)
            {
                case SepaChargeBearer.CRED:
                    return "CRED";
                case SepaChargeBearer.DEBT:
                    return "DEBT";
                case SepaChargeBearer.SHAR:
                    return "SHAR";
                default:
                    throw new ArgumentException("Unknown Charge Bearer : " + seqTp);
            }
        }

        /// <summary>
        ///     Get the Enum value from XML valid value for SeqTp
        /// </summary>
        /// <param name="seqTp">XML valid value for SeqTp</param>
        /// <returns>Enum value from SepaChargeBearer</returns>
        public static SepaChargeBearer SepaChargeBearerFromString(string seqTp)
        {
            switch (seqTp)
            {
                case "CRED":
                    return SepaChargeBearer.CRED;
                case "DEBT":
                    return SepaChargeBearer.DEBT;
                case "SHAR":
                    return SepaChargeBearer.SHAR;
                default:
                    throw new ArgumentException("Unknown Charge Bearer : " + seqTp);
            }
        }
    }
}