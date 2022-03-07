using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.Helpers;
using SimplCommerce.Infrastructure.Web;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Core.Services;
using SimplCommerce.Module.PaymentERede.Areas.PaymentERede.ViewModels;
using SimplCommerce.Module.PaymentERede.Models;
using SimplCommerce.Module.PaymentERede.Services;
using SimplCommerce.Module.Payments.Models;
using SimplCommerce.Module.ShoppingCart.Services;

namespace SimplCommerce.Module.PaymentERede.Areas.PaymentERede.Components
{
    public class ERedeLandingViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;
        private readonly IWorkContext _workContext;
        private readonly IRepositoryWithTypedId<PaymentProvider, string> _paymentProviderRepository;
        private readonly IERedeConfiguration _eredeConfiguration;
        private readonly ICurrencyService _currencyService;

        public ERedeLandingViewComponent(ICartService cartService,
            IWorkContext workContext,
            IRepositoryWithTypedId<PaymentProvider, string> paymentProviderRepository,
            IERedeConfiguration eredeConfiguration,
            ICurrencyService currencyService)
        {
            _cartService = cartService;
            _workContext = workContext;
            _paymentProviderRepository = paymentProviderRepository;
            _eredeConfiguration = eredeConfiguration;
            _currencyService = currencyService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var stripeProvider = await _paymentProviderRepository.Query().FirstOrDefaultAsync(x => x.Id == PaymentProviderHelper.ERedeProviderId);
            var stripeSetting = JsonConvert.DeserializeObject<ERedeConfigForm>(stripeProvider.AdditionalSettings);
            var curentUser = await _workContext.GetCurrentUser();
            var cart = await _cartService.GetActiveCartDetails(curentUser.Id);
            var zeroDecimalAmount = cart.OrderTotal;
            if (!CurrencyHelper.IsZeroDecimalCurrencies(_currencyService.CurrencyCulture))
            {
                zeroDecimalAmount *= 100;
            }

            var regionInfo = new RegionInfo(_currencyService.CurrencyCulture.LCID);
            var model = new ERedeCheckoutForm
            {
                ClientToken = await _eredeConfiguration.GetClientToken(),
                Amount = zeroDecimalAmount,
                ISOCurrencyCode = regionInfo.ISOCurrencySymbol
            };

            return View(this.GetViewPath(), model);
        }
    }
}
