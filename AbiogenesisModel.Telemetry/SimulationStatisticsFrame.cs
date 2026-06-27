using AbiogenesisModel.Telemetry.Interfaces;

namespace AbiogenesisModel.Telemetry;

public class SimulationStatisticsFrame
{
    private readonly IReadOnlyDictionary<Type, ISimulationStatistic> _statisticsByType;

    public long Tick { get; }

    public DateTime Timestamp { get; } = DateTime.UtcNow;

    public SimulationStatisticsFrame(long tick, IEnumerable<ISimulationStatistic> statistics)
    {
        Tick = tick;

        _statisticsByType = statistics.ToDictionary(statistic => statistic.GetType(), statistic => statistic);
    }

    public bool TryGet<TStatistic>(out TStatistic statistic)
        where TStatistic : class, ISimulationStatistic
    {
        if (_statisticsByType.TryGetValue(typeof(TStatistic), out var value))
        {
            statistic = (TStatistic)value;
            return true;
        }

        statistic = null!;
        return false;
    }

    public TStatistic GetRequired<TStatistic>()
        where TStatistic : class, ISimulationStatistic
    {
        if (TryGet<TStatistic>(out var statistic))
        {
            return statistic;
        }

        throw new InvalidOperationException($"Statistics frame does not contain statistic of type {typeof(TStatistic).FullName}.");
    }

    public IReadOnlyCollection<ISimulationStatistic> All => _statisticsByType.Values.ToArray();
}