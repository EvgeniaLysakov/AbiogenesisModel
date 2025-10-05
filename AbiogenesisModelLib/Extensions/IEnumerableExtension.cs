namespace AbiogenesisModel.Lib.Extensions;

public static class IEnumerableExtension
{
    public static IEnumerable<TSource> ExcludeNull<TSource>(this IEnumerable<TSource?> arr)
    {
        return arr.Where(t => t != null)!;
    }

    public static TSource[] ToArrayOrCast<TSource>(this IEnumerable<TSource> arr)
    {
        return arr as TSource[] ?? [.. arr];
    }

    public static string Join(this IEnumerable<string> arr, string separator = ", ")
    {
        return string.Join(separator, arr);
    }
}