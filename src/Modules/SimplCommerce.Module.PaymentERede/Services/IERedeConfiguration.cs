using System.Threading.Tasks;

namespace SimplCommerce.Module.PaymentERede.Services
{
    public interface IERedeConfiguration
    {
        string Environment { get; }

        string MerchantId { get; }

        string PublicKey { get; }

        string PrivateKey { get; }

        //Task<IBraintreeGateway> BraintreeGateway();
        Task<object> ERedeGateway();

        Task<string> GetClientToken();
    }
}
