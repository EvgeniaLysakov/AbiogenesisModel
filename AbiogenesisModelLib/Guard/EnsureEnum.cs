namespace AbiogenesisModel.Lib.Guard;

public static class EnsureEnum
{
    public static Ensure<T> IsInList<T>(this Ensure<T> ensure, IReadOnlyList<T> values, string? message = null) where T : Enum
    {
        if (!values.Contains(ensure.Value))
        {
            throw new InvalidOperationException(message ?? $"{ensure.Name} must be in the list [{string.Join(", ", values)}], but was '{ensure.Value}'.");
        }

        return ensure;
    }
}