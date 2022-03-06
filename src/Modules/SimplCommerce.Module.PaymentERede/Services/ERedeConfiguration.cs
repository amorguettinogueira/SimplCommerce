using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.PaymentERede.Areas.PaymentERede.ViewModels;
using SimplCommerce.Module.PaymentERede.Models;
using SimplCommerce.Module.Payments.Models;

namespace SimplCommerce.Module.PaymentERede.Services
{
    public class ERedeConfiguration : IERedeConfiguration
    {
        public string Environment { get; private set; }
        public string MerchantId { get; private set; }
        public string PublicKey { get; private set; }
        public string PrivateKey { get; private set; }

        //public async Task<IBraintreeGateway> BraintreeGateway()
        public async Task<object> ERedeGateway()
        {
            if (_eredeGateway == null)
            {
                _eredeGateway = await CreateGateway();
            }

            return _eredeGateway;
        }

        //private IBraintreeGateway _braintreeGateway { get; set; }
        private object _eredeGateway { get; set; }

        private readonly IRepositoryWithTypedId<PaymentProvider, string> _paymentProviderRepository;

        public ERedeConfiguration(IRepositoryWithTypedId<PaymentProvider, string> paymentProviderRepository)
        {
            _paymentProviderRepository = paymentProviderRepository;
        }


        //private async Task<IBraintreeGateway> CreateGateway()
        private async Task<object> CreateGateway()
        {
            var eredeProvider = await _paymentProviderRepository.Query().FirstOrDefaultAsync(x => x.Id == PaymentProviderHelper.ERedeProviderId);
            var eredeSetting = JsonConvert.DeserializeObject<ERedeConfigForm>(eredeProvider.AdditionalSettings);

            Environment = eredeSetting.Environment;
            MerchantId = eredeSetting.MerchantId;
            PublicKey = eredeSetting.PublicKey;
            PrivateKey = eredeSetting.PrivateKey;

            //return new BraintreeGateway(Environment, MerchantId, PublicKey, PrivateKey);
            return new { Environment, MerchantId, PublicKey, PrivateKey };
        }

        public async Task<string> GetClientToken()
        {
            //var gateway = await BraintreeGateway();
            var gateway = await ERedeGateway();
            //return await gateway.ClientToken.GenerateAsync();
            return nameof(gateway);
        }
    }
}
