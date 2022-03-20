using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Orders.Events;
using SimplCommerce.Module.Orders.Models;
using SimplCommerce.Module.PaymentERede.Areas.PaymentERede.ViewModels;
using SimplCommerce.Module.PaymentERede.Models;
using SimplCommerce.Module.Payments.Models;
using SimplCommerce.Module.PaymentsERede.Models;

namespace SimplCommerce.Module.PaymentERede.Areas.PaymentERede.Controllers
{
    [Area("PaymentERede")]
    [Authorize(Roles = "admin")]
    [Route("api/erede")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ERedeApiController : Controller
    {
        private readonly long SystemUserId = 2;
        private readonly IRepositoryWithTypedId<PaymentProvider, string> paymentProviderRepository;
        private readonly IRepository<ERedePayment> paymentERedeRepository;
        private readonly IRepository<Payment> paymentRepository;
        private readonly IRepository<Order> orderRepository;
        private readonly IMediator mediator;
        private readonly ILogger<ERedeApiController> logger;

        public ERedeApiController(
            IRepositoryWithTypedId<PaymentProvider, string> paymentProviderRepository,
            IRepository<ERedePayment> paymentERedeRepository,
            IRepository<Payment> paymentRepository,
            IRepository<Order> orderRepository,
            IMediator mediator,
            ILoggerFactory loggerFactory)
        {
            this.paymentProviderRepository = paymentProviderRepository;
            this.paymentERedeRepository = paymentERedeRepository;
            this.paymentRepository = paymentRepository;
            this.orderRepository = orderRepository;
            this.mediator = mediator;
            this.logger = loggerFactory.CreateLogger<ERedeApiController>();
        }

        [HttpGet("config")]
        public async Task<IActionResult> Config()
        {
            var stripeProvider = await paymentProviderRepository.Query().FirstOrDefaultAsync(x => x.Id == PaymentProviderHelper.ERedeProviderId);
            var model = JsonConvert.DeserializeObject<ERedeConfigForm>(stripeProvider.AdditionalSettings);
            return Ok(model);
        }

        [HttpPut("config")]
        public async Task<IActionResult> Config([FromBody] ERedeConfigForm model)
        {
            if (ModelState.IsValid)
            {
                var stripeProvider = await paymentProviderRepository.Query().FirstOrDefaultAsync(x => x.Id == PaymentProviderHelper.ERedeProviderId);
                stripeProvider.AdditionalSettings = JsonConvert.SerializeObject(model);
                await paymentProviderRepository.SaveChangesAsync();
                return Accepted();
            }

            return BadRequest(ModelState);
        }

        private async Task<ERedePayment> LogFormContentAsync()
        {
            var payment = new ERedePayment()
            {
                reference = Request.Form["reference"],
                tid = Request.Form["tid"],
                nsu = Request.Form["nsu"],
                authorizationCode = Request.Form["authorizationCode"],
                expiresAt = Request.Form["expiresAt"],
                date = Request.Form["date"],
                time = Request.Form["time"],
                returnCode = Request.Form["returnCode"],
                returnMessage = Request.Form["returnMessage"],
                avsReturnCode = Request.Form["avs.returnCode"],
                avsReturnMessage = Request.Form["avs.returnMessage"],
                threeDSecureReturnCode = Request.Form["threeDSecure.returnCode"],
                threeDSecureReturnMessage = Request.Form["threeDSecure.returnMessage"],
                brandReturnCode = Request.Form["brand.returnCode"],
                brandReturnMessage = Request.Form["brand.returnMessage"],
                brandName = Request.Form["brand.name"],
            };

            paymentERedeRepository.Add(payment);
            await paymentERedeRepository.SaveChangesAsync();
            return payment;
        }

        [HttpPost("sc")]
        public async Task<IActionResult> SuccessAsync() => await ProcessCallback();

        [HttpPost("fl")]
        public async Task<IActionResult> Failure() => await ProcessCallback();

        public async Task<IActionResult> ProcessCallback()
        {
            //Request.Log2Console("Success in", "Success out");
            var paymentRede = await LogFormContentAsync();

            var OrderId = paymentRede.reference.OnlyDigits();

            Order order = null;

            if (!string.IsNullOrEmpty(OrderId))
            {
                order = orderRepository.Query().Where(x => x.Id == long.Parse(OrderId)).FirstOrDefault();
                if (order != null)
                {
                    using var transaction = orderRepository.BeginTransaction();

                    bool success = false;

                    try
                    {
                        var oldStatus = order.OrderStatus;
                        order.OrderStatus = paymentRede.returnCode == "00" ? OrderStatus.PaymentReceived : OrderStatus.PaymentFailed;
                        order.OrderNote = paymentRede.returnCode == "00" ? string.Empty : $"Mensagem de retorno eRede: {paymentRede.returnMessage}";

                        var orderStatusChanged = new OrderChanged
                        {
                            OrderId = order.Id,
                            OldStatus = oldStatus,
                            NewStatus = order.OrderStatus,
                            UserId = SystemUserId,
                            Order = order
                        };

                        var payment = new Payment()
                        {
                            OrderId = order.Id,
                            PaymentFee = order.PaymentFeeAmount,
                            Amount = order.OrderTotal,
                            PaymentMethod = PaymentProviderHelper.ERedeProviderId,
                            CreatedOn = DateTimeOffset.UtcNow,
                            Status = order.OrderStatus == OrderStatus.PaymentReceived ? PaymentStatus.Succeeded : PaymentStatus.Failed,
                            GatewayTransactionId = paymentRede.tid
                        };

                        try
                        {
                            paymentRepository.Add(payment);
                            await paymentRepository.SaveChangesAsync();

                            await mediator.Publish(orderStatusChanged);
                            await orderRepository.SaveChangesAsync();
                            success = true;
                        }
                        catch (Exception e)
                        {
                            logger.LogError(0, e, "eRede callback exception, Order ID: {OrderId}, Transaction ID: {tid}", OrderId, paymentRede.tid);
                        }
                    }
                    finally
                    {
                        if (success) { transaction.Commit(); }
                        else { transaction.Rollback(); }
                    }

                    return Redirect($"~/user/orders/{OrderId}");
                }
            }

            logger.LogWarning(0, "eRede callback could not get target order, Transaction ID: {tid}, Order ID: {OrderId}, Order: {order}", paymentRede.tid, OrderId, order != null ? "null" : order.Id);
            return Redirect($"/");
        }
    }
}
