using AbiogenesisModel.Telemetry;
using AbiogenesisModel.Telemetry.Interfaces;

namespace AbiogenesisModel.App.Statistics;

public interface IStatisticsHistoryStore
{
    void AddFrame(SimulationStatisticsFrame frame);

    IReadOnlyList<StatisticSample<TStatistic>> GetSeries<TStatistic>()
        where TStatistic : class, ISimulationStatistic;

    bool TryGetLatest<TStatistic>(out StatisticSample<TStatistic> sample)
        where TStatistic : class, ISimulationStatistic;

    void Clear();
}