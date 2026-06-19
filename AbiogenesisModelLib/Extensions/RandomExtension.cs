using AbiogenesisModel.Lib.Guard;

namespace AbiogenesisModel.Lib.Extensions;

public static class RandomExtension
{
    public static T Choice<T>(this Random random, IReadOnlyList<T> list)
    {
        Ensure.That(list).IsNotEmpty();

        return list[random.Next(list.Count)];
    }

    public static double NextNormal(this Random random, double mean, double variance)
    {
        Ensure.That(variance).IsGreaterThan(0);

        var stdDev = Math.Sqrt(variance);

        // Box–Muller transform
        var u1 = 1.0 - random.NextDouble(); // (0,1]
        var u2 = 1.0 - random.NextDouble();

        var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);

        var value = mean + stdDev * randStdNormal;

        return value;
    }

    public static T[] NextArray<T>(this Random random, int length, Dictionary<T, double> probabilities)
    where T : notnull
    {
        Ensure.That(length).IsGreaterOrEqual(0);
        Ensure.That(probabilities).IsNotNull().IsNotEmpty().Satisfies(dict => Math.Abs(dict.Values.Sum() - 1) < 1E-3);

        var valuesArr = new (T value, double partialSum)[probabilities.Count];
        var partialSum = 0.0;
        var keys = probabilities.Keys.ToArray();
        for (var i = 0; i < valuesArr.Length; i++)
        {
            var key = keys[i];
            partialSum += probabilities[key];
            valuesArr[i] = (key, partialSum);
        }

        valuesArr[^1].partialSum = 1; // to avoid precision issues

        var array = new T[length];
        for (var i = 0; i < length; i++)
        {
            var rand = random.NextDouble();
            array[i] = valuesArr.First(pair => pair.partialSum >= rand).value;
        }

        return array;
    }
}