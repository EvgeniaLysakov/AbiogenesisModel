using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AbiogenesisModel.App.Chart;

public abstract class BaseStatisticChartViewModel : INotifyPropertyChanged, IBaseStatisticChartViewModel
{
    private string _title = string.Empty;
    private ChartKind? _kind;
    private ChartAxis? _xAxis;
    private ChartAxis? _yAxis;
    private IReadOnlyList<ChartSeries>? _series;

    public string Title
    {
        get => _title;
        protected set => SetField(ref _title, value);
    }

    public ChartKind? Kind
    {
        get => _kind;
        protected set => SetField(ref _kind, value);
    }

    public ChartAxis? XAxis
    {
        get => _xAxis;
        protected set => SetField(ref _xAxis, value);
    }

    public ChartAxis? YAxis
    {
        get => _yAxis;
        protected set => SetField(ref _yAxis, value);
    }

    public IReadOnlyList<ChartSeries>? Series
    {
        get => _series;
        protected set => SetField(ref _series, value);
    }

    public void Clear()
    {
        Series = null;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}