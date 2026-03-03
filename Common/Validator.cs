using System.Text.RegularExpressions;

namespace NexusGearMS.Common
{
    public static class Validator
    {
        /// <summary>
        /// Check if string is not null or whitespace
        /// </summary>
        public static bool IsRequired(string s)
        {
            return !string.IsNullOrWhiteSpace(s);
        }

        /// <summary>
        /// Validate phone number (9-15 digits)
        /// </summary>
        public static bool IsPhone(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return true; // Optional field
            return Regex.IsMatch(s, @"^\d{9,15}$");
        }

        /// <summary>
        /// Validate code format (A-Z, 0-9, dash, 3-20 chars)
        /// </summary>
        public static bool IsCode(string s)
        {
            return Regex.IsMatch(s ?? "", @"^[A-Z0-9\-]{3,20}$");
        }

        /// <summary>
        /// Validate positive number
        /// </summary>
        public static bool IsPositive(decimal value)
        {
            return value > 0;
        }

        /// <summary>
        /// Validate non-negative number
        /// </summary>
        public static bool IsNonNegative(decimal value)
        {
            return value >= 0;
        }

        /// <summary>
        /// Validate password strength (minimum 8 characters)
        /// </summary>
        public static bool IsValidPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && password.Length >= 8;
        }

        /// <summary>
        /// Check if quantity is valid (positive integer)
        /// </summary>
        public static bool IsValidQuantity(int qty)
        {
            return qty > 0;
        }
    }
}
