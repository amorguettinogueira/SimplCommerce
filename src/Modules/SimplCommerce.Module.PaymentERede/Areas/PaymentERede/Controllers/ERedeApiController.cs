using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.Orders.Services;
using SimplCommerce.Module.PaymentERede.Areas.PaymentERede.ViewModels;
using SimplCommerce.Module.PaymentERede.Models;
using SimplCommerce.Module.Payments.Models;

namespace SimplCommerce.Module.PaymentERede.Areas.PaymentERede.Controllers
{
    [Area("PaymentERede")]
    [Authorize(Roles = "admin")]
    [Route("api/erede")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ERedeApiController : Controller
    {
        private readonly IRepositoryWithTypedId<PaymentProvider, string> _paymentProviderRepository;
        private readonly IOrderService orderService;

        public ERedeApiController(
            IRepositoryWithTypedId<PaymentProvider, string> paymentProviderRepository,
            IOrderService orderService)
        {
            _paymentProviderRepository = paymentProviderRepository;
            this.orderService = orderService;
        }

        [HttpGet("config")]
        public async Task<IActionResult> Config()
        {
            var stripeProvider = await _paymentProviderRepository.Query().FirstOrDefaultAsync(x => x.Id == PaymentProviderHelper.ERedeProviderId);
            var model = JsonConvert.DeserializeObject<ERedeConfigForm>(stripeProvider.AdditionalSettings);
            return Ok(model);
        }

        [HttpPut("config")]
        public async Task<IActionResult> Config([FromBody] ERedeConfigForm model)
        {
            if (ModelState.IsValid)
            {
                var stripeProvider = await _paymentProviderRepository.Query().FirstOrDefaultAsync(x => x.Id == PaymentProviderHelper.ERedeProviderId);
                stripeProvider.AdditionalSettings = JsonConvert.SerializeObject(model);
                await _paymentProviderRepository.SaveChangesAsync();
                return Accepted();
            }

            return BadRequest(ModelState);
        }

        [HttpPost("sc")]
        public async Task<IActionResult> Success()
        {
            Request.Log2Console("Success in", "Success out");

            string Id = string.Empty;

            if (Request.Form["returnCode"] == "00")
            {
                Id = Request.Form["reference"];
            }

            Id = Id.OnlyDigits();

            if (string.IsNullOrEmpty(Id))
            {
                return Redirect($"/");
            }

            //var payment = new Payment()
            //{
            //    OrderId = int.Parse(Id),
            //    PaymentFee = order.PaymentFeeAmount,
            //    Amount = order.OrderTotal,
            //    PaymentMethod = "Paypal Express",
            //    CreatedOn = DateTimeOffset.UtcNow,
            //};

            return Redirect($"~/user/orders/{Id}");
        }

        [HttpPost("fl")]
        public async Task<IActionResult> Failure()
        {
            //Request.Log2Console("Failure in", "Failure out");
            string Id = string.Empty;

            if (Request.Form["returnCode"] == "00")
            {
                Id = Request.Form["reference"];
            }

            if (string.IsNullOrEmpty(Id))
            {
                return Redirect($"/");
            }

            return Redirect($"~/user/orders/{Id.OnlyDigits()}");
        }
    }
}
