﻿using System;
using System.Collections.Generic;
using static lab1PSSC.Domain.CartItems;
using lab1PSSC.Domain;
using static LanguageExt.Prelude;
using LanguageExt;
using System.Threading.Tasks;

namespace lab1PSSC
{
    
    class Program
    {
        static List<UnvalidatedCustomerItem> listOfItemsCopy;

        private static void showItems(List<UnvalidatedCustomerItem> list)
        {
            foreach(var item in list)
            {
                Console.WriteLine(item.itemDetails());
            }

        }

        private static string checkItemExists(List<UnvalidatedCustomerItem> list, string cod)
        {
            foreach (var item in list)
            {
                if (item.itemCode.Equals(cod))
                {
                    return "Produsul exista!";

                }
            }
            return "Produsul nu exista sau cod gresit!";
        }

        private static string checkStock(List<UnvalidatedCustomerItem> list, string cod)
        {
            foreach (var item in list)
            {
                if (item.itemCode.Equals(cod) && Convert.ToInt64(item.itemQuantity) > 0)
                {
                    return $"Produsul este in stoc! Stoc:{item.itemQuantity}";

                }
            }
            return "Produsul nu exista sau cod gresit!";
        }
        private static string checkAddress(List<UnvalidatedCustomerItem> list, string address)
        {
            foreach (var item in list)
            {
                if (item.address.Equals(address))
                {
                    return $"Adresa introdusa este corecta! {item.itemDetails()}";

                }
            }
            return "Adresa invalida!";
        }

        static async Task Main(string[] args) {
            int opt = int.MaxValue;
            var listOfItems = ReadListOfItems();
            listOfItemsCopy = new List<UnvalidatedCustomerItem>(listOfItems);
            listOfItems.ToArray();
            showItems(listOfItemsCopy);
            PayItemsCommand command = new(listOfItems);
            PaidItemWorkflow workflow = new PaidItemWorkflow();
            var result = await workflow.ExecuteAsync(command, CheckItemExists);
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

            do {
                Console.WriteLine("0.Exit");
                Console.WriteLine("1.Verificare produs");
                Console.WriteLine("2.Verificare stoc");
                Console.WriteLine("3.Verificare adresa");
                Console.WriteLine("alege: ");
                opt = Convert.ToInt32(Console.ReadLine());

                switch(opt) {
                    case 1: string cod = "";
                            Console.WriteLine("codul produsului cautat: ");
                            cod = Console.ReadLine();
                            //Console.WriteLine(checkItemExists(listOfItemsCopy, cod));
                            var item = ItemRegistrationNumber.TryParse(cod);
                            var itemExists = await item.Match(
                                Some: item => CheckItemExists(item).Match(Succ: value => value, exception => false),
                                None: () => Task.FromResult(false)
                            );
                            if (itemExists)
                            {
                                Console.WriteLine("Itemul cautat exista!");
                            }
                            else
                            {
                                Console.WriteLine("Nu exista itemul cautat!");
                            }
                        break;
                    case 2:
                            string verificareStoc = "";
                            Console.WriteLine("verificare stock pentru cod produs: ");
                            verificareStoc = Console.ReadLine();
                            // Console.WriteLine(checkStock(listOfItemsCopy, verificareStoc));
                            var quantity = ItemRegistrationNumber.TryParse(verificareStoc);
                            var quantityCheck = await quantity.Match(
                                    Some: quantity => CheckItemStock(quantity).Match(Succ: value => value, exception => false),
                                    None: () => Task.FromResult(false)
                                );
                            if (quantityCheck)
                            {
                                Console.WriteLine("Stoc disponibil!");
                            }
                            else
                            {
                                Console.WriteLine("Stoc epuizat!");
                            }
                        break;
                    case 3:
                            string verificareAdresa = "";
                            Console.WriteLine("introduceti adresa: ");
                            verificareAdresa = Console.ReadLine();
                            // Console.WriteLine(checkAddress(listOfItemsCopy, verificareAdresa));
                            var address = Address.TryParseAddress(verificareAdresa);
                            var addressCheck = await address.Match(
                                    Some: address => CheckItemAddress(address).Match(Succ: value => value, exception => false),
                                    None: () => Task.FromResult(false)
                                );
                            if (addressCheck)
                            {
                                Console.WriteLine("Adresa valida!");
                            }
                            else
                            {
                                Console.WriteLine("Adresa invalida!");
                            }
                        break;
                }
            } while (opt != 0);

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

           //showItems(listOfItems);
            return listOfItems;
        
        }

        private static string? ReadValue(string prompt) { 
            Console.Write(prompt);
            return Console.ReadLine();
        }

        private static TryAsync<bool> CheckItemExists(ItemRegistrationNumber itemNumber)
        {
            Func<Task<bool>> func = async () =>
            {
                foreach (var item in listOfItemsCopy)
                {
                    if (item.itemCode.Equals(itemNumber.ToString()))
                    {
                        return true;
                    }
                }
                return false;
            };
            return TryAsync(func);
        }

        private static TryAsync<bool> CheckItemStock(ItemRegistrationNumber itm)
        {
            Func<Task<bool>> func = async () =>
            {
                foreach (var item in listOfItemsCopy)
                {
                    if (item.itemCode.Equals(itm.ToString()))
                    {
                        if (Convert.ToInt32(item.itemQuantity) > 0)
                        {
                            return true;
                        }
                    }
                }
                return false;
            };
            return TryAsync(func);
        }

        private static TryAsync<bool> CheckItemAddress(Address address)
        {
            Func<Task<bool>> func = async () =>
            {
                foreach (var item in listOfItemsCopy)
                {
                    if (item.address.Equals(address.ToString()))
                    {
                        return true;
                    }
                }
                return false;
            };
            return TryAsync(func);
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


