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
        private static readonly Regex ValidPattern = new("^LM[0-9]{5}$");

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

        public override string ToString()
        {
            return Value;
        }
    }
}
