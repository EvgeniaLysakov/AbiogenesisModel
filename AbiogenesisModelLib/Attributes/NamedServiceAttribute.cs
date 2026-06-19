using Microsoft.Extensions.DependencyInjection;

namespace AbiogenesisModel.Lib.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class NamedServiceAttribute(string key, ServiceLifetime lifetime = ServiceLifetime.Transient) : ServiceAttribute(lifetime)
{
    public string Key { get; } = key;
}