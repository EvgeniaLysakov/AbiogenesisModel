using System.Collections;

namespace AbiogenesisModel.Lib.Guard;

public static class EnsureCollection
{
    public static Ensure<TCollection> IsNotEmpty<TCollection>(this Ensure<TCollection> ensure, string? message = null)
        where TCollection : IEnumerable?
    {
        var value = ensure.Value;
        var name = ensure.Name;

        switch (value)
        {
            case null:
                throw new ArgumentNullException(name, message ?? $"{name} must be not null.");

            case string { Length: 0 }:
            case Array { Length: 0 }:
            case ICollection { Count: 0 }:
                break;

            default:
                var enumerator = value.GetEnumerator();
                using (enumerator as IDisposable)
                {
                    if (enumerator.MoveNext())
                    {
                        return ensure;
                    }

                    break;
                }

        }

        throw new ArgumentException(message ?? $"{name} must not be empty.", name);
    }

    public static Ensure<IReadOnlyList<T>> IsNullFree<T>(this Ensure<IReadOnlyList<T>> ensure, string? message = null) where T : class
    {
        var list = ensure.Value;

        foreach (var item in list)
        {
            if (item is null)
            {
                throw new ArgumentException(message ?? $"{ensure.Name} must not contain null elements.", ensure.Name);
            }
        }

        return ensure;
    }

    public static Ensure<IReadOnlyList<T>> IsDuplicateFree<T>(this Ensure<IReadOnlyList<T>> ensure, string? message = null)
        where T : class
    {
        var list = ensure.Value;

        if (list.Count <= 1)
        {
            return ensure;
        }

        var seen = new HashSet<T>(ReferenceEqualityComparer.Instance);

        for (var i = 0; i < list.Count; i++)
        {
            if (!seen.Add(list[i]))
            {
                throw new ArgumentException(message ?? $"{ensure.Name} must not contain duplicate references (duplicate at index {i}).", ensure.Name);
            }
        }

        return ensure;
    }
}