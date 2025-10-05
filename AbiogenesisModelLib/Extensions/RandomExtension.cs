namespace AbiogenesisModel.Lib.Extensions;

public static class RandomExtension
{
    public static T Choice<T>(this Random random, IReadOnlyList<T> list)
    {
        if (list?.Any() != true)
        {
            throw new ArgumentException("The list cannot be null or empty", nameof(list));
        }

        return list[random.Next(list.Count)];
    }
}