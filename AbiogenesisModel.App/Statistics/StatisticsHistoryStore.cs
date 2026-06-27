using AbiogenesisModel.Telemetry;
using AbiogenesisModel.Telemetry.Interfaces;

namespace AbiogenesisModel.App.Statistics;

public sealed class StatisticsHistoryStore(int maxSamplesPerStatistic = 10_000) : IStatisticsHistoryStore
{
    private readonly Dictionary<Type, List<StatisticSample>> _seriesByType = new();

    public void AddFrame(SimulationStatisticsFrame frame)
    {
        foreach (var statistic in frame.All)
        {
            var type = statistic.GetType();

            if (!_seriesByType.TryGetValue(type, out var series))
            {
                series = [];
                _seriesByType.Add(type, series);
            }

            series.Add(new StatisticSample(frame.Tick, frame.Timestamp, statistic));

            if (series.Count > maxSamplesPerStatistic)
            {
                series.RemoveAt(0);
            }
        }
    }

    public IReadOnlyList<StatisticSample<TStatistic>> GetSeries<TStatistic>()
        where TStatistic : class, ISimulationStatistic
    {
        if (!_seriesByType.TryGetValue(typeof(TStatistic), out var series))
        {
            return Array.Empty<StatisticSample<TStatistic>>();
        }

        return series.Select(sample => new StatisticSample<TStatistic>(sample.Tick, sample.Timestamp, (TStatistic)sample.Statistic)).ToArray();
    }

    public bool TryGetLatest<TStatistic>(out StatisticSample<TStatistic> sample)
        where TStatistic : class, ISimulationStatistic
    {
        if (_seriesByType.TryGetValue(typeof(TStatistic), out var series) && series.Count > 0)
        {
            var last = series[^1];

            sample = new StatisticSample<TStatistic>(last.Tick, last.Timestamp, (TStatistic)last.Statistic);

            return true;
        }

        sample = null!;
        return false;
    }

    public void Clear()
    {
        _seriesByType.Clear();
    }
}