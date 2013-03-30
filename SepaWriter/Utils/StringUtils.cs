using System;
using System.Globalization;

namespace Perrich.SepaWriter.Utils
{
    /// <summary>
    ///     Some Utilities to manage strings
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        ///     Get string with only the allowed length (truncate in other case)
        /// </summary>
        /// <param name="value">The string to check</param>
        /// <param name="allowedLength">The max allowed length</param>
        /// <returns></returns>
        public static string GetLimitedString(string value, int allowedLength)
        {

            if (value != null && value.Length > allowedLength)
            {
                return value.Substring(0, allowedLength);
            }
            return value;
        }

        /// <summary>
        ///     Format an amount with optional two decimals
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static string FormatAmount(decimal amount)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:0.##}", amount);
        }

        /// <summary>
        ///     Format a date with time using ISO 8601
        /// </summary>
        /// <param name="date">The date to format</param>
        /// <returns></returns>
        public static string FormatDateTime(DateTime date)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-dd\\THH:mm:ss}", date);
        }

        /// <summary>
        ///     Format a date using ISO 8601
        /// </summary>
        /// <param name="date">The date to format</param>
        /// <returns></returns>
        public static string FormatDate(DateTime date)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-dd}", date);
        }
    }
}
