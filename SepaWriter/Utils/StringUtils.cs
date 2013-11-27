using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Perrich.SepaWriter.Utils
{
    /// <summary>
    ///     Some Utilities to manage strings
    /// </summary>
    public static class StringUtils
    {
        private static readonly Regex CleanUpRegex = new Regex("[^A-Z0-9@/\\-?:()\\. ,'\"+]");

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

        /// <summary>
        ///     Allow to remove invalid character in ISO 20022
        /// </summary>
        /// <param name="str">The string to clean</param>
        /// <returns>The string without invalid character</returns>
        public static String RemoveInvalidChar(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            // remove french "accent"
            byte[] bytes = Encoding.GetEncoding(1251).GetBytes(str);
            str = Encoding.ASCII.GetString(bytes).ToUpper();

            // Remove invalid char
            return CleanUpRegex.Replace(str, " ");
        }
    }
}