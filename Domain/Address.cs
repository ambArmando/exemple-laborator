using System.Text.RegularExpressions;

namespace lab1PSSC.Domain
{
    public record Address
    {
        private static readonly Regex ValidPattern = new("^RO");

        public string Value { get; }

        private Address(string value)
        {
            if (ValidPattern.IsMatch(value))
            {
                Value = value;
            }
            else
            {
                throw new InvalidAddressException("");
            }
        }
        public static bool ValidateInputAddress(string address, out Address address1)
        {
            bool isValid = false;
            address1 = null;
            if (IsValid(address))
            {
                isValid = true;
                address1 = new(address);
            }
            return isValid;
        }

        private static bool IsValid(string stringValue) => ValidPattern.IsMatch(stringValue);

        public override string ToString()
        {
            return Value;
        }
    }
}