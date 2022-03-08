﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using eRede;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.Helpers;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Core.Services;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.Orders.Services;
using SimplCommerce.Module.PaymentERede.Areas.PaymentERede.ViewModels;
using SimplCommerce.Module.PaymentERede.Models;
using SimplCommerce.Module.PaymentERede.Services;
using SimplCommerce.Module.Payments.Models;
using SimplCommerce.Module.ShoppingCart.Services;

namespace SimplCommerce.Module.PaymentERede.Areas.PaymentERede.Controllers
{
    [Authorize]
    [Area("PaymentERede")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ERedeController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;
        private readonly IRepositoryWithTypedId<PaymentProvider, string> _paymentProviderRepository;
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IERedeConfiguration _eredeConfiguration;
        private readonly ICurrencyService _currencyService;

        public ERedeController(
            ICartService cartService,
            IOrderService orderService,
            IWorkContext workContext,
            IRepositoryWithTypedId<PaymentProvider, string> paymentProviderRepository,
            IRepository<Payment> paymentRepository,
            IERedeConfiguration eredeConfiguration,
            ICurrencyService currencyService)
        {
            _cartService = cartService;
            _orderService = orderService;
            _workContext = workContext;
            _paymentProviderRepository = paymentProviderRepository;
            _paymentRepository = paymentRepository;
            _eredeConfiguration = eredeConfiguration;
            _currencyService = currencyService;
        }

        [AutoValidateAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Checkout([FromForm] string radioKind,
                                                  [FromForm] string cardNumber,
                                                  [FromForm] string cardName,
                                                  [FromForm] string cardExpiry,
                                                  [FromForm] string cardCvc)
        {
            var currentUser = await _workContext.GetCurrentUser();
            var cart = await _cartService.GetActiveCartDetails(currentUser.Id);

            if (cart == null)
            {
                return NotFound();
            }

            var MonthYear = cardExpiry.MonthYearSplit("/");

            if (!Validate(radioKind, ref cardNumber, ref cardName, MonthYear[ERedeExtensions.MONTH], MonthYear[ERedeExtensions.YEAR], ref cardCvc))
            {
                TempData["Error"] = "Payment Method is not eligible for this order.";
                return Redirect("~/checkout/payment");
            }

            var orderCreateResult = await _orderService.CreateOrder(cart.Id, PaymentProviderHelper.ERedeProviderId, 0);

            if (!orderCreateResult.Success)
            {
                TempData["Error"] = orderCreateResult.Error;
                return Redirect("~/checkout/payment");
            }

            var redeProvider = await _paymentProviderRepository.Query().FirstOrDefaultAsync(x => x.Id == PaymentProviderHelper.ERedeProviderId);
            var redeSetting = JsonConvert.DeserializeObject<ERedeConfigForm>(redeProvider.AdditionalSettings);

            var store = new Store(redeSetting.RedePV, redeSetting.RedeToken, redeSetting.Sandbox ? eRede.Environment.Sandbox() : eRede.Environment.Production());

            // Transação que será autorizada
            var transaction = new Transaction { amount = decimal.ToInt32(orderCreateResult.Value.OrderTotal * 100), reference = $"PED {orderCreateResult.Value.Id}" };
            if (radioKind[0] == ERedeExtensions.CREDIT)
                transaction.CreditCard(cardNumber, cardCvc, MonthYear[ERedeExtensions.MONTH], MonthYear[ERedeExtensions.YEAR], cardName);
            else
                transaction.DebitCard(cardNumber, cardCvc, MonthYear[ERedeExtensions.MONTH], MonthYear[ERedeExtensions.YEAR], cardName);

            transaction.threeDSecure = new ThreeDSecure
            {
                embedded = true,
                onFailure = ThreeDSecure.CONTINUE_ON_FAILURE
            };

            transaction.AddUrl(Request.GetEndpoint("api/erede/sc/"), eRede.Url.THREE_D_SECURE_SUCCESS);
            transaction.AddUrl(Request.GetEndpoint("api/erede/fl/"), eRede.Url.THREE_D_SECURE_FAILURE);

            // Autoriza a transação
            var response = new eRede.eRede(store).create(transaction);

            if (response.returnCode == "220")
            {
                Console.Write(response.threeDSecure.url);
            }
            else
            if (response.returnCode == "00")
            {
                Console.WriteLine("Transação autorizada com sucesso: " + response.tid);
            }

            return Redirect($"~/checkout/success?orderId={orderCreateResult.Value.Id}");
        }

        public bool Validate(string radioKind,
                             ref string cardNumber,
                             ref string cardName,
                             string cardMonth,
                             string cardYear,
                             ref string cardCvc)
        {
            cardNumber = cardNumber.OnlyDigits();
            cardName = string.IsNullOrEmpty(cardName) ? string.Empty : cardName.ToUpper().Trim();
            cardCvc = cardCvc.OnlyDigits();
            return true;
        }

        [HttpPost]
        public async Task<IActionResult> Charge(string nonce)
        {
            var gateway = await _eredeConfiguration.ERedeGateway();

            var curentUser = await _workContext.GetCurrentUser();
            var cart = await _cartService.GetActiveCartDetails(curentUser.Id);
            if (cart == null)
            {
                return NotFound();
            }

            var orderCreateResult = await _orderService.CreateOrder(cart.Id, PaymentProviderHelper.ERedeProviderId, 0, OrderStatus.PendingPayment);

            if (!orderCreateResult.Success)
            {
                return BadRequest(orderCreateResult.Error);
            }

            var order = orderCreateResult.Value;
            var zeroDecimalOrderAmount = order.OrderTotal;
            if (!CurrencyHelper.IsZeroDecimalCurrencies(_currencyService.CurrencyCulture))
            {
                zeroDecimalOrderAmount *= 100;
            }

            var regionInfo = new RegionInfo(_currencyService.CurrencyCulture.LCID);
            var payment = new Payment()
            {
                OrderId = order.Id,
                Amount = order.OrderTotal,
                PaymentMethod = PaymentProviderHelper.ERedeProviderId,
                CreatedOn = DateTimeOffset.UtcNow
            };

            //var lineItemsRequest = new List<TransactionLineItemRequest>();
            var lineItemsRequest = new List<object>();

            //TODO: Need validation
            //foreach(var item in order.OrderItems)
            //{
            //    lineItemsRequest.Add(new TransactionLineItemRequest
            //    {
            //        Description = item.Product.Description.Substring(0, 255),
            //        Name = item.Product.Name,
            //        Quantity = item.Quantity,
            //        UnitAmount = item.ProductPrice,
            //        ProductCode = item.ProductId.ToString(),
            //        TotalAmount = item.ProductPrice * item.Quantity

            //    });
            //}

            //TODO: See how customer id works
            var request = new TransactionRequest
            {
                Amount = order.OrderTotal,
                PaymentMethodNonce = nonce,
                OrderId = order.Id.ToString(),
                //LineItems = lineItemsRequest.ToArray(),
                //CustomerId = order.CustomerId.ToString(),
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true,
                    SkipAdvancedFraudChecking = false,
                    SkipCvv = false,
                    SkipAvs = false,
                }
            };

            //var result = gateway.Transaction.Sale(request);
            //if (result.IsSuccess())
            if (request != null)
            {
                //var transaction = result.Target;

                payment.GatewayTransactionId = "1"; // transaction.Id;
                payment.Status = PaymentStatus.Succeeded;
                order.OrderStatus = OrderStatus.PaymentReceived;
                _paymentRepository.Add(payment);
                await _paymentRepository.SaveChangesAsync();

                return Ok(new { Status = "success", OrderId = order.Id, TransactionId = "1" /*transaction.Id*/ });
            }
            else
            {
                string errorMessages = "";
                //foreach (var error in result.Errors.DeepAll())
                //{
                //    errorMessages += "Error: " + (int)error.Code + " - " + error.Message + "\n";
                //}

                return BadRequest(errorMessages);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetClientToken()
        {
            return Ok(await _eredeConfiguration.GetClientToken());
        }

        private class TransactionRequest
        {
            public decimal Amount { get; set; }
            public string PaymentMethodNonce { get; set; }
            public string OrderId { get; set; }
            public object Options { get; set; }
        }

        private class TransactionOptionsRequest
        {
            public bool SubmitForSettlement { get; set; }
            public bool SkipAdvancedFraudChecking { get; set; }
            public bool SkipCvv { get; set; }
            public bool SkipAvs { get; set; }
        }
    }
}
