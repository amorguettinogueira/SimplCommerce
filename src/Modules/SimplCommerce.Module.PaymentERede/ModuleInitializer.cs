using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SimplCommerce.Infrastructure;
using SimplCommerce.Infrastructure.Modules;
using SimplCommerce.Module.PaymentERede.Services;

namespace SimplCommerce.Module.PaymentERede
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IERedeConfiguration, ERedeConfiguration>();

            GlobalConfiguration.RegisterAngularModule("simplAdmin.paymentERede");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
