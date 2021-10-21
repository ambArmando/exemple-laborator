using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace lab1PSSC.Domain
{
    public record ItemRegistrationNumber
    {
        private static readonly Regex ValidPattern = new("^pr[0-9]");

        public string Value { get; }

        private ItemRegistrationNumber(string value)
        {
            if (ValidPattern.IsMatch(value))
            {
                Value = value;
            }
            else
            {
                throw new InvalidItemRegistrationNumberException("");
            }
        }
        public static bool ValidateInputRegistrationNumber(string itemCode, out ItemRegistrationNumber itemc)
        {
            bool isValid = false;
            itemc = null;
            if (IsValid(itemCode)) {
                isValid = true;
                itemc = new(itemCode);
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
