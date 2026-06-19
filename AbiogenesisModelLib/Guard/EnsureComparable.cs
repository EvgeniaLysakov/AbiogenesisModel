using System.Collections;
using System.Runtime.CompilerServices;

namespace AbiogenesisModel.Lib.Guard;

public static class EnsureComparable
{
    public static Ensure<T> IsGreaterThan<T>(this Ensure<T> ensure, T min, string? message = null) where T : IComparable<T>
    {
        if (ensure.Value.CompareTo(min) <= 0)
        {
            throw new ArgumentOutOfRangeException(ensure.Name, ensure.Value, message ?? $"{ensure.Name} must be greater than '{min}', but was '{ensure.Value}'.");
        }

        return ensure;
    }

    public static Ensure<T> IsGreaterOrEqual<T>(this Ensure<T> ensure, T min, string? message = null) where T : IComparable<T>
    {
        if (ensure.Value.CompareTo(min) < 0)
        {
            throw new ArgumentOutOfRangeException(ensure.Name, ensure.Value, message ?? $"{ensure.Name} must be greater than or equal to '{min}', but was '{ensure.Value}'.");
        }

        return ensure;
    }

    public static Ensure<T> IsLessThan<T>(this Ensure<T> ensure, T max, string? message = null) where T : IComparable<T>
    {
        if (ensure.Value.CompareTo(max) >= 0)
        {
            throw new ArgumentOutOfRangeException(ensure.Name, ensure.Value, message ?? $"{ensure.Name} must be less than '{max}', but was '{ensure.Value}'.");
        }

        return ensure;
    }

    public static Ensure<T> IsLessOrEqual<T>(this Ensure<T> ensure, T max, string? message = null) where T : IComparable<T>
    {
        if (ensure.Value.CompareTo(max) > 0)
        {
            throw new ArgumentOutOfRangeException(ensure.Name, ensure.Value, message ?? $"{ensure.Name} must be less than or equal to '{max}', but was '{ensure.Value}'.");
        }

        return ensure;
    }

    public static Ensure<T> IsInRange<T>(this Ensure<T> ensure, T min, T max, string? message = null) where T : IComparable<T>
    {
        if (ensure.Value.CompareTo(min) < 0 || ensure.Value.CompareTo(max) > 0)
        {
            throw new ArgumentOutOfRangeException(ensure.Name, ensure.Value, message ?? $"{ensure.Name} must be in range [{min}, {max}], but was '{ensure.Value}'.");
        }

        return ensure;
    }

    public static Ensure<int> IsInRangeOfIndexes(this Ensure<int> ensure, IEnumerable collection, [CallerArgumentExpression("collection")] string collectionName = "???", string? message = null)
    {
        const int min = 0;
        var max = Math.Max(0, collection.Cast<object>().Count() - 1);
        if (ensure.Value.CompareTo(min) < 0 || ensure.Value.CompareTo(max) > 0)
        {
            throw new ArgumentOutOfRangeException(ensure.Name, ensure.Value, message ?? $"{ensure.Name} must be in range of indexes of {collectionName}({min}, {max}), but was '{ensure.Value}'.");
        }

        return ensure;
    }
}