using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1PSSC.Domain
{
    public record Item
    {
        public int Value { get; }

        public Item(int quantity)
        {
            if (IsValid(quantity))
            {
                Value = quantity;
            }
            else
            {
                throw new InvalidItemException($"{quantity:0.##} is an invalid quantity value.");
            }
        }
        public override string ToString()
        {
            return $"{Value:0.##}";
        }

        public static bool TryParseItemQuantity(string itemqString, out Item itemq)
        {
            bool isValid = false;
            itemq = null;
            if (int.TryParse(itemqString, out int numericItemq)) {
                if (IsValid(numericItemq)) {
                    isValid = true;
                    itemq = new(numericItemq);
                }
            }
            return isValid;
        }

        private static bool IsValid(int numericItemq) => numericItemq > 0 && numericItemq <= 100;
    }
}
