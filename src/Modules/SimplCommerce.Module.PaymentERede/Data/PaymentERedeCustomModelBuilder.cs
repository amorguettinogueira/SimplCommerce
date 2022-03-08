using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.PaymentERede.Models;
using SimplCommerce.Module.Payments.Models;

namespace SimplCommerce.Module.PaymentERede.Data
{
    public class PaymentERedeCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentProvider>().HasData(
                //new PaymentProvider("Braintree")
                new PaymentProvider(PaymentProviderHelper.ERedeProviderId)
                {
                    Name = PaymentProviderHelper.ERedeProviderId,
                    LandingViewComponentName = "ERedeLanding",
                    ConfigureUrl = "payments-erede-config",
                    IsEnabled = true,
                    AdditionalSettings =
                    "{" +
                        "\"Sandbox\": \"true\", " +
                        "\"RedePV\" : \"\", " +
                        "\"RedeToken\" : \"\", " +
                        "\"SoftDescriptor\" : \"\", " +
                        "\"QtdeParcelas\" : 0, " +
                        "\"ValorMinimoParcelamento\" : 0," +
                    "}"
                }
            );
        }
    }
}
