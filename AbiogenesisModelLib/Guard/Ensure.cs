using System.Runtime.CompilerServices;

namespace AbiogenesisModel.Lib.Guard;

public static class Ensure
{
    public static Ensure<T> That<T>(T value, [CallerArgumentExpression("value")] string paramName = "???")
    {
        return new Ensure<T>(value, paramName);
    }

    public static Ensure<T> ThatNamed<T>(T value, string paramName)
    {
        return new Ensure<T>(value, paramName);
    }
}

public readonly struct Ensure<T>
{
    internal Ensure(T value, string name)
    {
        Value = value;
        Name = name;
    }

    public T Value { get; }
    public string Name { get; }

    public Ensure<T> IsNotDefault(string? message = null)
    {
        if (EqualityComparer<T>.Default.Equals(Value, default))
        {
            throw new ArgumentNullException(Name, message ?? $"{Name} must be not default.");
        }

        return this;
    }

    public Ensure<T> IsDefault(string? message = null)
    {
        if (!EqualityComparer<T>.Default.Equals(Value, default))
        {
            throw new ArgumentException(message ?? $"{Name} must be default.", Name);
        }

        return this;
    }

    public Ensure<T> IsNotNull(string? message = null)
    {
        if (Value is null)
        {
            throw new ArgumentNullException(Name, message ?? $"{Name} must be not null.");
        }

        return this;
    }

    public Ensure<T> IsNull(string? message = null)
    {
        if (Value is not null)
        {
            throw new ArgumentException(message ?? $"{Name} must be null.", Name);
        }

        return this;
    }

    public Ensure<T> Satisfies(Func<T, bool> predicate, string? message = null)
    {
        if (!predicate(Value))
        {
            throw new ArgumentException(message ?? $"{Name} does not satisfy required condition.", Name);
        }

        return this;
    }

    public Ensure<T> DoesNotSatisfy(Func<T, bool> predicate, string? message = null)
    {
        if (predicate(Value))
        {
            throw new ArgumentException(message ?? $"{Name} must not satisfy given condition.", Name);
        }

        return this;
    }

    public Ensure<T> EqualsTo(T expected, string? message = null)
    {
        if (!EqualityComparer<T>.Default.Equals(Value, expected))
        {
            throw new ArgumentException(message ?? $"{Name} must be equal to '{expected}', but was '{Value}'.", Name);
        }

        return this;
    }

    public Ensure<T> DoesNotEqualTo(T other, string? message = null)
    {
        if (EqualityComparer<T>.Default.Equals(Value, other))
        {
            throw new ArgumentException(message ?? $"{Name} must not be equal to '{other}'.", Name);
        }

        return this;
    }

    public Ensure<T> IsKeyOf<TValue>(IReadOnlyDictionary<T, TValue> dictionary, [CallerArgumentExpression("dictionary")] string dictionaryName = "???", string? message = null)
    {
        var isIncluded = dictionary.ContainsKey(Value);
        if (!isIncluded)
        {
            throw new ArgumentException(message ?? $"{Name} must be included in keys collection of {dictionaryName}.", Name);
        }

        return this;
    }

    public Ensure<T> IsNotKeyOf<TValue>(IReadOnlyDictionary<T, TValue> dictionary, [CallerArgumentExpression("dictionary")] string dictionaryName = "???", string? message = null)
    {
        var isIncluded = dictionary.ContainsKey(Value);
        if (isIncluded)
        {
            throw new ArgumentException(message ?? $"{Name} must not be included in keys collection of {dictionaryName}.", Name);
        }

        return this;
    }
}