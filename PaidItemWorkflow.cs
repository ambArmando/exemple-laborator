﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static lab1PSSC.Domain.CartItems;
using static lab1PSSC.Domain.CartItemsPaidEvent;
using lab1PSSC.Domain;
using static lab1PSSC.Domain.CartItemsOperations;
using LanguageExt;
using System.Threading.Tasks;
using lab1PSSCAmbrusArmando.Repositories;
using Microsoft.Extensions.Logging;
using static LanguageExt.Prelude;

namespace lab1PSSC
{
    class PaidItemWorkflow
    {
        private readonly ProductRepository productRepository;
        private readonly OrderHeaderRepository orderHeader;
        private readonly ILogger<PaidItemWorkflow> logger;
        public PaidItemWorkflow(ProductRepository productRepository, OrderHeaderRepository orderHeaderRepository, ILogger<PaidItemWorkflow> logger)
        {
            this.productRepository = productRepository;
            this.orderHeader = orderHeaderRepository;
            this.logger = logger;
        }

        public async Task<ICartItemsPaidEvent> ExecuteAsync(PayItemsCommand command)
        {

            UnvalidatedCartItems unvalidatedCartItems = new UnvalidatedCartItems(command.InputCartItems);

            var result = from items in productRepository.TryGetExistingStudents(unvalidatedCartItems.ItemList.Select(item => item.itemCode))
                         .ToEither(ex => new FailedCartItem(unvalidatedCartItems.ItemList, ex) as ICartItems)
                         from existingItems in orderHeader.TryGetExistingItems().ToEither(ex => new FailedCartItem(unvalidatedCartItems.ItemList, ex) as ICartItems)
                         let checkItemExist = (Func<ItemRegistrationNumber, Option<ItemRegistrationNumber>>)(item => CheckItemExists(items, item))
                         from paidItems in ExecuteWorkflowAsync(unvalidatedCartItems, existingItems, checkItemExist).ToAsync()
                         select paidItems;

            return await result.Match(
                Left: items => GenerateFailedEvent(items) as ICartItemsPaidEvent,
                Right: rItems => new CartItemsSucceededPayEvent(rItems.Csv, rItems.ItemList.ToString())
                );
        }

        private async Task<Either<ICartItems, PaidCartItems>> ExecuteWorkflowAsync(UnvalidatedCartItems unvalidatedItems,
                                                                                   IEnumerable<ItemFinalPrice> existingItems, Func<ItemRegistrationNumber,
                                                                                   Option<ItemRegistrationNumber>> checkItemExists)
        {
            ICartItems items = await ValidateCartItems(checkItemExists, unvalidatedItems);
            items = CalculatePrice(items);
            items = MergeItems(items, existingItems);
            items = PublishCartItems(items);

            return items.Match<Either<ICartItems, PaidCartItems>>(
                whenEmptyCartItems: empty => Left(empty as ICartItems),
                whenUnvalidatedCartItems: unvalidatedCartItems => Left(unvalidatedCartItems as ICartItems),
                whenInvalidatedCartItems: invalidCart => Left(invalidCart as ICartItems),
                whenFailedCartItem: failed => Left(failed as ICartItems),
                whenValidatedCartItems: validCart => Left(validCart as ICartItems),
                whenCalculatedItemPrice: calculated => Left(calculated as ICartItems),
                whenPaidCartItems: paidCartItems => Right(paidCartItems)
                );

        }

        private Option<ItemRegistrationNumber> CheckItemExists(IEnumerable<ItemRegistrationNumber> items, ItemRegistrationNumber itemRegistrationNumber)
        {
            if (items.Any(s => s == itemRegistrationNumber))
            {
                return Some(itemRegistrationNumber);
            }
            else
            {
                return None;
            }
        }

        private CartItemsFailedPayEvent GenerateFailedEvent(ICartItems items) =>
            items.Match<CartItemsFailedPayEvent>(
                whenEmptyCartItems: empty => new ($"Invalid state {nameof(EmptyCartItems)}"),
                whenUnvalidatedCartItems: unvalidatedCartItems => new($"Invalid state {nameof(UnvalidatedCartItems)}"),
                whenInvalidatedCartItems: invalidCart => new(invalidCart.Reason),
                whenValidatedCartItems: validCart => new($"Invalid state {nameof(ValidatedCartItems)}"),
                whenFailedCartItem: failed => { logger.LogError(failed.Exception, failed.Exception.Message); return new(failed.Exception.Message); },
                whenCalculatedItemPrice: calculated => new($"Invalid state {nameof(CalculatedItemPrice)}"),
                whenPaidCartItems: paidCartItems => new($"Invalid state {nameof(PaidCartItems)}"));


        //public async Task<ICartItemsPaidEvent> ExecuteAsync(PayItemsCommand command, Func<ItemRegistrationNumber, TryAsync<bool>> checkItemExists) {

        //    UnvalidatedCartItems unvalidatedCartItems = new UnvalidatedCartItems(command.InputCartItems);
        //    ICartItems items = await ValidateCartItems(checkItemExists, unvalidatedCartItems);
        //    items = CalculatePrice(items);
        //    items = PayCartItems(items);

        //    return items.Match(
        //            whenEmptyCartItems: emptyCart => new CartItemsFailedPayEvent("Empty cart state") as ICartItemsPaidEvent,
        //            whenUnvalidatedCartItems: unvalidatedCartItems => new CartItemsFailedPayEvent("Unexpected unvalidated state"),
        //            whenInvalidatedCartItems: invalidCart => new CartItemsFailedPayEvent(invalidCart.Reason),
        //            whenValidatedCartItems: validCart => new CartItemsFailedPayEvent("Unexpected validated state"),
        //            whenCalculatedItemPrice: calculated => new CartItemsFailedPayEvent("Unexpected calculated state"),
        //            whenPaidCartItems: paidCartItems => new CartItemsSucceededPayEvent(paidCartItems.Csv, paidCartItems.ItemList.ToString())
        //        );
        //}
    }
}
