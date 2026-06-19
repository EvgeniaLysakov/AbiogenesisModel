using System.Runtime.CompilerServices;

namespace AbiogenesisModel.Lib.Guard;

public static class EnsureReferenceTypes
{
    public static Ensure<T> IsNotSameAs<T>(this Ensure<T> ensure, T other, [CallerArgumentExpression("other")] string otherParamName = "???", string? message = null)
        where T : class
    {
        if (ReferenceEquals(ensure.Value, other))
        {
            throw new ArgumentException(message ?? $"{ensure.Name} must not reference the same object as {otherParamName}.", ensure.Name);
        }

        return ensure;
    }
}