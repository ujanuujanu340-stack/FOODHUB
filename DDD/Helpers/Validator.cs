using System.Text.RegularExpressions;

namespace DDD.Helpers
{
    /// <summary>Small collection of reusable input-validation checks used by the Services layer.</summary>
    public static class Validator
    {
        public static bool IsNullOrEmpty(string value) => string.IsNullOrWhiteSpace(value);

        /// <summary>Digits only, allowing an optional leading + (for phone numbers).</summary>
        public static bool IsValidPhone(string value)
        {
            if (IsNullOrEmpty(value)) return false;
            return Regex.IsMatch(value.Trim(), @"^\+?[0-9]{7,15}$");
        }

        /// <summary>Sri Lankan style NIC: 9 digits + V/X, or 12 digits.</summary>
        public static bool IsValidNIC(string value)
        {
            if (IsNullOrEmpty(value)) return false;
            return Regex.IsMatch(value.Trim().ToUpper(), @"^([0-9]{9}[VX]|[0-9]{12})$");
        }

        public static bool IsValidDecimal(string value)
        {
            return decimal.TryParse(value, out _);
        }

        public static bool IsValidInt(string value)
        {
            return int.TryParse(value, out _);
        }

        public static bool IsPositiveDecimal(string value)
        {
            return decimal.TryParse(value, out decimal d) && d >= 0;
        }

        public static bool IsPositiveInt(string value)
        {
            return int.TryParse(value, out int i) && i >= 0;
        }
    }
}
