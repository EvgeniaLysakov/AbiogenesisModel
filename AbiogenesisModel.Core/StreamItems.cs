namespace AbiogenesisModel.Core
{
    public interface IStreamItem { }

    public sealed record TextItem(string Text) : IStreamItem;

    public abstract record PlotItem(string? Title = null, string? XTitle = null, string? YTitle = null) : IStreamItem;

    public sealed record LineData((double X, double Y)[] Dots, string? Legend = null);

    public sealed record LinePlotItem(LineData[] Lines) : PlotItem;

    public sealed record BarPlotItem((double X, double Value)[] Bars) : PlotItem;

    public sealed record LabeledBarPlotItem((string X, double Value)[] Bars) : PlotItem;
}
