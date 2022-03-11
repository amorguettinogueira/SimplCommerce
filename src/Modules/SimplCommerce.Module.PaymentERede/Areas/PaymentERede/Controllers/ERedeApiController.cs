using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SimplCommerce.Infrastructure.Data;
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

        public ERedeApiController(IRepositoryWithTypedId<PaymentProvider, string> paymentProviderRepository)
        {
            _paymentProviderRepository = paymentProviderRepository;
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
            Console.WriteLine("Success in");
            Log(Request);
            Console.WriteLine("Success out");
            return BadRequest(ModelState);
        }

        private void Log(HttpRequest Request)
        {
            foreach (var item in Request.Query)
            {
                Console.WriteLine(
                    $"Query {item.Key} = {item.Value}"
                    );
            }
            foreach (var item in Request.Headers)
            {
                Console.WriteLine(
                    $"Headers {item.Key} = {item.Value}"
                    );
            }
            foreach (var item in Request.RouteValues)
            {
                Console.WriteLine(
                    $"RouteValues {item.Key} = {item.Value}"
                    );
            }
            foreach (var item in Request.Form)
            {
                Console.WriteLine(
                    $"Form {item.Key} = {item.Value}"
                    );
            }
            foreach (var item in Request.Form)
            {
                Console.WriteLine(
                    $"Form {item.Key} = {item.Value}"
                    );
            }
            Console.WriteLine($"QueryString {Request.QueryString}");
            Console.WriteLine($"ToString() {Request}");
        }

        [HttpPost("fl")]
        public async Task<IActionResult> Failure()
        {
            Console.WriteLine("Failure in");
            Log(Request);
            Console.WriteLine("Failure out");
            return BadRequest(ModelState);
        }
    }
}
