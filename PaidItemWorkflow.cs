using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static lab1PSSC.Domain.CartItems;
using static lab1PSSC.Domain.CartItemsPaidEvent;
using lab1PSSC.Domain;
using static lab1PSSC.Domain.CartItemsOperations;

namespace lab1PSSC
{
    class PaidItemWorkflow
    {
        public ICartItemsPaidEvent Execute(PayItemsCommand command, Func<ItemRegistrationNumber, bool> checkItemExists) {

            UnvalidatedCartItems unvalidatedCartItems = new UnvalidatedCartItems(command.InputCartItems);
            ICartItems items = ValidateCartItems(checkItemExists, unvalidatedCartItems);
            items = CalculatePrice(items);
            items = PayCartItems(items);

            return items.Match(
                    whenEmptyCartItems: emptyCart => new CartItemsFailedPayEvent("Empty cart state") as ICartItemsPaidEvent,
                    whenUnvalidatedCartItems: unvalidatedCartItems => new CartItemsFailedPayEvent("Unexpected unvalidated state"),
                    whenInvalidatedCartItems: invalidCart => new CartItemsFailedPayEvent(invalidCart.Reason),
                    whenValidatedCartItems: validCart => new CartItemsFailedPayEvent("Unexpected validated state"),
                    whenCalculatedItemPrice: calculated => new CartItemsFailedPayEvent("Unexpected calculated state"),
                    whenPaidCartItems: paidCartItems => new CartItemsSucceededPayEvent(paidCartItems.Csv, paidCartItems.ItemList.ToString())
                );
        }
    }
}
