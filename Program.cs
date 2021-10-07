using System;
using System.Collections.Generic;
using static lab1PSSC.Domain.CartItems;
using lab1PSSC.Domain;

namespace lab1PSSC
{
    class Program
    {
        private static readonly Random random = new Random();
        static List<UnvalidatedCustomerItem> listOfItems;
        public static Payment payment;
        public static PaymentState ps;
        private static string adr;
        static void Main(string[] args)
        {
            var listOfItems = ReadListOfItems().ToArray();
            UnvalidatedCartItems unvalidatedCartItems = new(listOfItems);

            ICartItems result = CartState(unvalidatedCartItems);

            result.Match(
                whenEmptyCartItems: emptyCartItems => emptyCartItems,
                whenUnvalidatedCartItems: unvalidateResult => unvalidatedCartItems,
                whenPaidCartItems: paidItem => paidItem,
                whenInvalidatedCartItems: invalidResult => invalidResult,
                whenValidatedCartItems: validatedResult => PayCartItems(validatedResult)
            );

            Console.WriteLine(result);
            ShowCartItems();
        }

        private static List<UnvalidatedCustomerItem> ReadListOfItems() {

           listOfItems = new();

            do {
                var registrationNumber = ReadValue("Registration Number: ");
                if (string.IsNullOrEmpty(registrationNumber))
                {
                    break;
                }

                var quantity = ReadValue("Quantity: ");
                if (string.IsNullOrEmpty(quantity))
                {
                    break;
                }

                var address = ReadValue("Address: ");
                if (string.IsNullOrEmpty(address))
                {
                    break;
                }
                adr = address;

                var paid = ReadValue("paid: [y/n]: ");
                if (string.IsNullOrEmpty(paid))
                {
                    break;
                }

                if (paid.Equals("y"))
                {
                    payment = new Payment("y");
                }
                else {
                    payment = new Payment("n");
                }
                ps = new PaymentState(payment);
                listOfItems.Add(new(registrationNumber, quantity, address, paid));
            } while (true);

            return listOfItems;
        }

        private static ICartItems CartState(UnvalidatedCartItems item) {
            if (item.ItemsList.Count == 0)
            {
               return new EmptyCartItems(new List<UnvalidatedCustomerItem>(), "empty cart");
            }
            else if (payment.ToString().Equals("y"))
            {
                return new PaidCartItems(new List<ValidatedCustomerItem>(), adr);
            }
            else {
               return new ValidatedCartItems(new List<ValidatedCustomerItem>());
            }
            return new UnvalidatedCartItems(new List<UnvalidatedCustomerItem>());
        }

        private static ICartItems PayCartItems(ValidatedCartItems validated) {
           return new PaidCartItems(new List<ValidatedCustomerItem>(), "adresa random");   
        }
           
        private static void ShowCartItems() {
            foreach (var item in listOfItems) {
                Console.WriteLine(item);
            }
        }
       
        private static string? ReadValue(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

    }
}
