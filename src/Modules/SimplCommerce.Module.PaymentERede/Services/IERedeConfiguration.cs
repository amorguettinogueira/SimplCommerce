using System.Threading.Tasks;

namespace SimplCommerce.Module.PaymentERede.Services
{
    public interface IERedeConfiguration
    {
        bool Sandbox { get; }

        string RedePV { get; }

        int QtdeParcelas { get; }

        double ValorMinimoParcelamento { get; }

        string RedeToken { get; }

        string SoftDescriptor { get; }

        //Task<IBraintreeGateway> BraintreeGateway();
        Task<object> ERedeGateway();

        Task<string> GetClientToken();
    }
}
