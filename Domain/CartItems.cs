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

        public record UnvalidatedCartItems : ICartItems { 
            public UnvalidatedCartItems(IReadOnlyCollection<UnvalidatedCustomerItem> itemList)
            {
                ItemList = itemList;
            }
            public IReadOnlyCollection<UnvalidatedCustomerItem> ItemList { get; }
        }

        public record InvalidatedCartItems : ICartItems
        {
            internal InvalidatedCartItems(IReadOnlyCollection<UnvalidatedCustomerItem> itemList, string reason) {
                ItemList = itemList;
                Reason = reason;
            }
            public IReadOnlyCollection<UnvalidatedCustomerItem> ItemList { get; }
            public string Reason { get; }
            
        }

        public record ValidatedCartItems : ICartItems
        {
            internal ValidatedCartItems(IReadOnlyCollection<ValidatedCustomerItem> itemList)
            {
                ItemList = itemList;
            }
            public IReadOnlyCollection<ValidatedCustomerItem> ItemList { get; }
        }

        public record PaidCartItems : ICartItems {
            internal PaidCartItems(IReadOnlyCollection<ValidatedCustomerItem> itemList, string address, string csv)
            {
                ItemList = itemList;
                Address = address;
                Csv = csv;
            }
            public IReadOnlyCollection<ValidatedCustomerItem> ItemList { get; }
            public string Address { get; }
            public string Csv { get; }
        }
    }
}
