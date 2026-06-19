using Microsoft.Extensions.DependencyInjection;

namespace AbiogenesisModel.Lib.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute(ServiceLifetime lifetime = ServiceLifetime.Singleton) : Attribute
{
    public ServiceLifetime Lifetime { get; } = lifetime;
}