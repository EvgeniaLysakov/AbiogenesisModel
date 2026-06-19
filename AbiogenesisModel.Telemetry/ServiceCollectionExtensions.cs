using AbiogenesisModel.Lib.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AbiogenesisModel.Telemetry;

public static class ServiceCollectionExtensions
{
    public static void RegisterTelemetryServices(this IServiceCollection services)
    {
        services.RegisterServices(Assembly.GetExecutingAssembly());
    }
}