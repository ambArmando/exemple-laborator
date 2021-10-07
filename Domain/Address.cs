using System.Text.RegularExpressions;

namespace lab1PSSC.Domain
{
    public record Address
    {
        private static readonly Regex ValidPattern = new("^LM[0-9]{5}$");

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

        public override string ToString()
        {
            return Value;
        }
    }
}