using AbiogenesisModel.Telemetry.Interfaces;

namespace AbiogenesisModel.App.Statistics;

public sealed record StatisticSample(
    long Tick,
    DateTime Timestamp,
    ISimulationStatistic Statistic);

public sealed record StatisticSample<TStatistic>(
    long Tick,
    DateTime Timestamp,
    TStatistic Statistic)
    where TStatistic : class, ISimulationStatistic;