using AbiogenesisModel.App.Statistics;
using AbiogenesisModel.Telemetry.Interfaces;

namespace AbiogenesisModel.App.Chart;

public static class StatisticChartViewModelExtension
{
    public static bool TryToUpdate<TStatistic>(this IBaseStatisticChartViewModel chartViewModel, IReadOnlyList<StatisticSample<TStatistic>> samples)
        where TStatistic : class, ISimulationStatistic
    {
        switch (chartViewModel)
        {
            case IStatisticHistoryChartViewModel<TStatistic> historyDataModel:
                historyDataModel.Update(samples);
                return true;
            case ILastStatisticChartViewModel<TStatistic> lastStatisticDataModel:
                lastStatisticDataModel.Update(samples.Last().Statistic);
                return true;
            case null:
                return false;
            default:
                throw new ArgumentException("Unsupported data model type.", nameof(chartViewModel));
        }
    }
}