using Microsoft.Extensions.DependencyInjection;

namespace AbiogenesisModel.Lib.Pipeline;

public sealed class ConfigFactory
{
    public required Dictionary<string, string> ConfigKeys { get; init; }
}

public interface IConfigFactory<out T>
{
    T Get();
}

public sealed class ConfigFactory<T>(IServiceProvider sp, ConfigFactory configFactory) : IConfigFactory<T>
    where T : notnull
{
    public T Get()
    {
        configFactory.ConfigKeys.TryGetValue(typeof(T).Name, out var key);
        return sp.GetRequiredKeyedService<T>(key ?? "default");
    }
}
