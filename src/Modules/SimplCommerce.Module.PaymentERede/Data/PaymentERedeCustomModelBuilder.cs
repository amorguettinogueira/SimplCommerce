using Microsoft.EntityFrameworkCore;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Module.PaymentERede.Models;
using SimplCommerce.Module.Payments.Models;
using SimplCommerce.Module.PaymentsERede.Models;

namespace SimplCommerce.Module.PaymentERede.Data
{
    public class PaymentERedeCustomModelBuilder : ICustomModelBuilder
    {
        public void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ERedePayment>().HasIndex(x => x.reference);

            modelBuilder.Entity<PaymentProvider>().HasData(
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
