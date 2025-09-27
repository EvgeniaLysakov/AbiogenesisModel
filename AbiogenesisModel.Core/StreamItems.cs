namespace AbiogenesisModel.Core
{
    public interface IStreamItem { }

    public sealed record TextItem(string Text) : IStreamItem;

    public abstract record PlotItem(string? Title = null, string? XTitle = null, string? YTitle = null) : IStreamItem;

    public sealed record LinePlotItem(double[] X, double[] Y) : PlotItem;

    public sealed record BarPlotItem((string Label, double Value)[] Bars) : PlotItem;
}
