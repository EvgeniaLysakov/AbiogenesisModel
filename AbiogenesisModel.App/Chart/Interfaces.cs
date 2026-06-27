using AbiogenesisModel.App.Statistics;
using AbiogenesisModel.Telemetry.Interfaces;
using System.ComponentModel;

namespace AbiogenesisModel.App.Chart;

public interface IBaseStatisticChartViewModel
{
    string Title { get; }

    ChartKind? Kind { get; }

    ChartAxis? XAxis { get; }

    ChartAxis? YAxis { get; }

    IReadOnlyList<ChartSeries>? Series { get; }

    void Clear();

    event PropertyChangedEventHandler? PropertyChanged;
}

public interface ILastStatisticChartViewModel<in TStatistic> : IBaseStatisticChartViewModel
    where TStatistic : class, ISimulationStatistic
{
    void Update(TStatistic statistic);
}

public interface IStatisticHistoryChartViewModel<TStatistic> : IBaseStatisticChartViewModel
    where TStatistic : class, ISimulationStatistic
{
    void Update(IReadOnlyList<StatisticSample<TStatistic>> samples);
}