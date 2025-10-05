using Microsoft.Extensions.DependencyInjection;

namespace AbiogenesisModel.Lib.Pipeline;

public interface IKeyedFactory<out T>
{
    T Get(object? key);
}

public sealed class KeyedFactory<T>(IServiceProvider sp) : IKeyedFactory<T>
    where T : notnull
{
    public T Get(object? key)
    {
        return sp.GetRequiredKeyedService<T>(key ?? "default");
    }
}