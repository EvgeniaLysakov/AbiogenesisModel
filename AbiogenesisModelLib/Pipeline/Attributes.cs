using Microsoft.Extensions.DependencyInjection;

namespace AbiogenesisModel.Lib.Pipeline;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute(ServiceLifetime lifetime = ServiceLifetime.Singleton) : Attribute
{
    public ServiceLifetime Lifetime { get; } = lifetime;
}

[AttributeUsage(AttributeTargets.Class)]
public class NamedServiceAttribute(string key, ServiceLifetime lifetime = ServiceLifetime.Transient) : ServiceAttribute(lifetime)
{
    public string Key { get; } = key;
}

[AttributeUsage(AttributeTargets.Class)]
public class ConfigAttribute(string? directoryName = null) : Attribute
{
    public string? DirectoryName { get; } = directoryName;
}