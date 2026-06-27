using AbiogenesisModel.App.Statistics;
using AbiogenesisModel.Telemetry.Statistics;

namespace AbiogenesisModel.App.Chart;

public sealed class StrandTotalOverTimeViewModel : BaseStatisticChartViewModel, IStatisticHistoryChartViewModel<StrandStatistic>
{
    public StrandTotalOverTimeViewModel()
    {
        Title = "Total strands over time";
        Kind = ChartKind.Line;
        XAxis = new ChartAxis("Tick", ChartAxisKind.Tick);
        YAxis = new ChartAxis("Population", ChartAxisKind.Numeric);
    }

    public void Update(IReadOnlyList<StatisticSample<StrandStatistic>> samples)
    {
        var points = samples
            .Select(sample => new ChartPoint(
                X: sample.Tick,
                Y: sample.Statistic.TotalCount,
                Label: sample.Tick.ToString()))
            .ToArray();

        Series = [new ChartSeries("Total strands", points)];
    }
}