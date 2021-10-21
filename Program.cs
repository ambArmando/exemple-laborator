using System;
using System.Collections.Generic;
using static lab1PSSC.Domain.CartItems;
using lab1PSSC.Domain;


namespace lab1PSSC
{
    class Program
    {
        static void Main(string[] args) {
            var listOfItems = ReadListOfItems().ToArray();
            PayItemsCommand command = new(listOfItems);
            PaidItemWorkflow workflow = new PaidItemWorkflow();
            var result = workflow.Execute(command, (registrationCode) => true);

            result.Match(
                    whenCartItemsFailedPayEvent: @event => {
                        Console.WriteLine($"Publish failed {@event.Reason}");
                        return @event;
                    },
                    whenCartItemsSucceededPayEvent: @event => {
                        Console.WriteLine("Publish succeeded");
                        Console.WriteLine(@event.Csv);
                        return @event;
                    }
                );

        }
        private static List<UnvalidatedCustomerItem> ReadListOfItems() {

            List<UnvalidatedCustomerItem> listOfItems = new();

            do {

                var itemCode = ReadValue("item code: ");
                if (string.IsNullOrEmpty(itemCode)) {
                    break;
                }

                var itemQuantity = ReadValue("item quantity: ");
                if (string.IsNullOrEmpty(itemQuantity))
                {
                    break;
                }

                var address = ReadValue("address: ");
                if (string.IsNullOrEmpty(address))
                {
                    break;
                }

                var paid = ReadValue("paid[y/n]: ");
                if (string.IsNullOrEmpty(paid))
                {
                    break;
                }

                listOfItems.Add(new(itemCode, itemQuantity, address, paid));

            } while(true);

            return listOfItems;
        
        }

        private static string? ReadValue(string prompt) { 
            Console.Write(prompt);
            return Console.ReadLine();
        }

    }
}



//    static List<UnvalidatedCustomerItem> listOfItems;
//    public static Payment payment;
//    public static PaymentState ps;
//    private static string adr;
//    static void Main(string[] args)
//    {
//        var listOfItems = ReadListOfItems().ToArray();
//        UnvalidatedCartItems unvalidatedCartItems = new(listOfItems);
//        ICartItems result = CartState(unvalidatedCartItems);
//        result.Match(
//            whenEmptyCartItems: emptyCartItems => emptyCartItems,
//            whenUnvalidatedCartItems: unvalidateResult => unvalidatedCartItems,
//            whenPaidCartItems: paidItem => paidItem,
//            whenInvalidatedCartItems: invalidResult => invalidResult,
//            whenValidatedCartItems: validatedResult => validatedResult//PayCartItems(validatedResult)
//        );

//        Console.WriteLine(result);
//        //ShowCartItems();
//    }

//    private static List<UnvalidatedCustomerItem> ReadListOfItems() {

//       listOfItems = new();

//        do {
//            var registrationNumber = ReadValue("Registration Number: ");
//            if (string.IsNullOrEmpty(registrationNumber))
//            {
//                break;
//            }

//            var quantity = ReadValue("Quantity: ");
//            if (string.IsNullOrEmpty(quantity))
//            {
//                break;
//            }

//            var address = ReadValue("Address: ");
//            if (string.IsNullOrEmpty(address))
//            {
//                break;
//            }
//            adr = address;

//            var paid = ReadValue("paid: [y/n]: ");
//            if (string.IsNullOrEmpty(paid))
//            {
//                break;
//            }

//            if (paid.Equals("y"))
//            {
//                payment = new Payment("y");
//            }
//            else {
//                payment = new Payment("n");
//            }
//            ps = new PaymentState(payment);
//            listOfItems.Add(new(registrationNumber, quantity, address, paid));
//        } while (true);

//        return listOfItems;
//    }

//    private static ICartItems CartState(UnvalidatedCartItems item) {
//        //Console.WriteLine(ps.payment.GetType());
//        //Console.WriteLine(ps.payment.ToString().GetType());
//        if (item.ItemsList.Count == 0)
//        {
//            Console.WriteLine("---Emptry cart state---");
//           return new EmptyCartItems(new List<UnvalidatedCustomerItem>(), "empty cart");
//        }
//        else if (ps.payment.ToString().Equals("y"))
//        {
//            Console.WriteLine("---PAID cart state---");
//            return new PaidCartItems(new List<ValidatedCustomerItem>(), adr);
//        }
//        else if (listOfItems.Count > 0){
//            Console.WriteLine("---Valid cart state---");
//            return new ValidatedCartItems(new List<ValidatedCustomerItem>());
//        }
//        Console.WriteLine("---Unvalid cart state---");
//        return new UnvalidatedCartItems(new List<UnvalidatedCustomerItem>());
//    }

//    private static ICartItems PayCartItems(ValidatedCartItems validated) {
//       return new PaidCartItems(new List<ValidatedCustomerItem>(), "adresa random");   
//    }

//    private static void ShowCartItems() {
//        foreach (var item in listOfItems) {
//            Console.WriteLine(item);
//        }
//    }


