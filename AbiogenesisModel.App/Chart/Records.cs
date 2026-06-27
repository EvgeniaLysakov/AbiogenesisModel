namespace AbiogenesisModel.App.Chart;

public sealed record ChartPoint(
    double X,
    double Y,
    string? Label = null);

public sealed record ChartSeries(
    string Name,
    IReadOnlyList<ChartPoint> Points);

public sealed record ChartAxis(
    string Title,
    ChartAxisKind Kind);