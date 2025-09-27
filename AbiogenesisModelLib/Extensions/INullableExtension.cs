namespace AbiogenesisModel.Lib.Extensions
{
    public static class INullableExtension
    {
        public static bool IsNullOrBigger<T>(this T? obj, T val)
            where T : struct, IComparable<T>
        {
            return !obj.HasValue || obj.Value.CompareTo(val) > 0;
        }
    }
}
