using System;

namespace Perrich.SepaWriter.Utils
{
    public static class SepaBatchBookingUtils
    {
        /// <summary>
        ///     Get awaited XML string from Enum value
        /// </summary>
        /// <param name="sbbTp">Enum value</param>
        /// <returns>Awaited XML string</returns>
        public static string SepaBatchBookingToString(SepaBatchBooking sbbTp)
        {
            switch (sbbTp)
            {
                case SepaBatchBooking.MTM:
                    return "false";
                case SepaBatchBooking.MTO:
                    return "true";
                default:
                    throw new ArgumentException("Unknown Batch Booking : " + sbbTp);
            }
        }

        /// <summary>
        ///     Get the Enum value from XML valid value for SbbTp
        /// </summary>
        /// <param name="sbbTp">XML valid value for SbbTp</param>
        /// <returns>Enum value from SepaBatchBooking</returns>
        public static SepaBatchBooking SepaBatchBookingFromString(string sbbTp)
        {
            switch (sbbTp)
            {
                case "false":
                    return SepaBatchBooking.MTM;
                case "true":
                    return SepaBatchBooking.MTO;
                default:
                    throw new ArgumentException("Unknown Batch Booking : " + sbbTp);
            }
        }
    }
}