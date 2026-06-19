namespace AbiogenesisModel.Lib.Guard;

public static class EnsureString
{
    public static Ensure<string> IsNotNullOrEmptyString(this Ensure<string> ensure, string? message = null)
    {
        if (string.IsNullOrEmpty(ensure.Value))
        {
            throw new ArgumentException(message ?? $"{ensure.Name} must not be null or empty.", ensure.Name);
        }

        return ensure;
    }

    public static Ensure<string> IsNotNullOrWhiteSpace(this Ensure<string> ensure, string? message = null)
    {
        if (string.IsNullOrWhiteSpace(ensure.Value))
        {
            throw new ArgumentException(message ?? $"{ensure.Name} must not be null or whitespace.", ensure.Name);
        }

        return ensure;
    }
}