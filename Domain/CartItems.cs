using System;
using CSharp.Choices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1PSSC.Domain
{
    [AsChoice]
    public static partial class CartItems
    {
        public interface ICartItems { }

        public record EmptyCartItems(IReadOnlyCollection<UnvalidatedCustomerItem> ItemsList, string empty) : ICartItems;

        public record UnvalidatedCartItems(IReadOnlyCollection<UnvalidatedCustomerItem> ItemsList) : ICartItems;

        public record InvalidatedCartItems(IReadOnlyCollection<UnvalidatedCustomerItem> ItemsList, string reason) : ICartItems;

        public record ValidatedCartItems(IReadOnlyCollection<ValidatedCustomerItem> ItemsList) : ICartItems;

        public record PaidCartItems(IReadOnlyCollection<ValidatedCustomerItem> ItemsList, string address) : ICartItems;
    }
}
