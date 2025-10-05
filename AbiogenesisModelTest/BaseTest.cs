using AbiogenesisModel.Lib.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace AbiogenesisModel.Test
{
    public class BaseTest
    {
        protected ServiceProvider InitServiceCollection(string generalConfigPath = "config\\general.yml")
        {
            var services = new ServiceCollection();
            services.RegisterGeneralConfigFromFile(generalConfigPath);
            services.RegisterConfigs();
            services.RegisterServices();

            return services.BuildServiceProvider();
        }
    }
}
