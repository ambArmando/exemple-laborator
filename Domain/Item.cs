using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1PSSC.Domain
{
    public record Item
    {
        public decimal Value { get; }

        public Item(decimal quantity)
        {
            if (quantity > 0 && quantity <= 100)
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
    }
}
