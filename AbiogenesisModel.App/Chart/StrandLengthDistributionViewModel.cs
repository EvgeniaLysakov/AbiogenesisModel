using AbiogenesisModel.Telemetry.Statistics;

namespace AbiogenesisModel.App.Chart;

public sealed class StrandLengthDistributionViewModel : BaseStatisticChartViewModel, ILastStatisticChartViewModel<StrandStatistic>
{
    public StrandLengthDistributionViewModel()
    {
        Title = "Strand length distribution";
        Kind = ChartKind.Bar;
        XAxis = new ChartAxis("Strand length", ChartAxisKind.Numeric);
        YAxis = new ChartAxis("Count", ChartAxisKind.Numeric);
    }

    public void Update(StrandStatistic statistic)
    {
        var points = statistic.PerLengthCount
            .OrderBy(pair => pair.Key)
            .Select(pair => new ChartPoint(
                X: pair.Key,
                Y: pair.Value,
                Label: pair.Key.ToString()))
            .ToArray();

        Series = [new ChartSeries("Strands", points)];
    }
}