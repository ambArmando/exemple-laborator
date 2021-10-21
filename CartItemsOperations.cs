using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static lab1PSSC.Domain.CartItems;

namespace lab1PSSC.Domain
{
    public static class CartItemsOperations 
    {
        public static ICartItems ValidateCartItems(Func<ItemRegistrationNumber, bool> checkItemExists, UnvalidatedCartItems cartItems)
        {
            List<ValidatedCustomerItem> validatedCartItems = new();
            bool isValidList = true;
            string invalidReason = string.Empty;
            foreach (var unvalidatedItem in cartItems.ItemList)
            {
                if (!Item.TryParseItemQuantity(unvalidatedItem.itemQuantity, out Item itemq)) {
                    invalidReason = $"Invalid item quantity ({unvalidatedItem.itemCode}, {unvalidatedItem.itemQuantity})";
                    isValidList = false;
                    break;
                }
                if (!ItemRegistrationNumber.ValidateInputRegistrationNumber(unvalidatedItem.itemCode, out ItemRegistrationNumber itemRegistration))
                {
                    invalidReason = $"Invalid item registration number ({unvalidatedItem.itemCode})";
                    isValidList = false;
                    break;
                }
                if (!Address.ValidateInputAddress(unvalidatedItem.address, out Address address))
                {
                    invalidReason = $"Invalid address ({unvalidatedItem.itemCode}, {unvalidatedItem.address})";
                    isValidList = false;
                    break;
                }
                if (!Payment.ValidateInputPayment(unvalidatedItem.paid, out Payment payment))
                {
                    invalidReason = $"Invalid payment ({unvalidatedItem.itemCode}, {unvalidatedItem.paid})";
                    isValidList = false;
                    break;
                }
                ValidatedCustomerItem validCommand = new(itemRegistration, itemq, address, payment);
                validatedCartItems.Add(validCommand);
            }

            if (isValidList)
            {
                return new ValidatedCartItems(validatedCartItems);
            }
            else
            {
                return new InvalidatedCartItems(cartItems.ItemList, invalidReason);
            }
        }
    }
}
