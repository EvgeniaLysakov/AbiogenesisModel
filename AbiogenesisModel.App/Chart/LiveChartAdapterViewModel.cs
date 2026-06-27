using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AbiogenesisModel.App.Chart;

public sealed class LiveChartAdapterViewModel : INotifyPropertyChanged
{
    private ISeries[] _series = [];
    private Axis[] _xAxes = [];
    private Axis[] _yAxes = [];

    public event PropertyChangedEventHandler? PropertyChanged;

    public ISeries[] Series
    {
        get => _series;
        private set => SetField(ref _series, value);
    }

    public Axis[] XAxes
    {
        get => _xAxes;
        private set => SetField(ref _xAxes, value);
    }

    public Axis[] YAxes
    {
        get => _yAxes;
        private set => SetField(ref _yAxes, value);
    }

    public void Update(IBaseStatisticChartViewModel? chartViewModel)
    {
        if (chartViewModel?.Series?.Any() != true)
        {
            XAxes = [];
            YAxes = [];
            Series = [];
            return;
        }

        XAxes = CreateXAxes(chartViewModel);

        YAxes =
        [
            new Axis
            {
                Name = chartViewModel.YAxis?.Title,
                MinStep = 1
            }
        ];

        Series = chartViewModel.Kind switch
        {
            ChartKind.Line => CreateLineSeries(chartViewModel),
            ChartKind.Bar => CreateColumnSeries(chartViewModel),
            ChartKind.Scatter => CreateScatterSeries(chartViewModel),
            _ => []
        };
    }

    private static Axis[] CreateXAxes(IBaseStatisticChartViewModel chart)
    {
        var firstSeries = chart.Series?.FirstOrDefault();

        if (chart.Kind == ChartKind.Bar && firstSeries is not null)
        {
            return
            [
                new Axis
                {
                    Name = chart.XAxis?.Title,
                    Labels = firstSeries.Points
                        .Select(point => point.Label ?? point.X.ToString("0"))
                        .ToArray(),

                    MinStep = 1,
                    ForceStepToMin = true,
                    UnitWidth = 1
                }
            ];
        }

        return
        [
            new Axis
            {
                Name = chart.XAxis?.Title,
                MinStep = chart.XAxis?.Kind == ChartAxisKind.Tick ? 1 : 0
            }
        ];
    }

    private static ISeries[] CreateLineSeries(IBaseStatisticChartViewModel chart)
    {
        return chart.Series?
            .Select(series => new LineSeries<double>
            {
                Name = series.Name,
                Values = series.Points.Select(point => point.Y).ToArray(),
                GeometrySize = 1
            })
            .Cast<ISeries>()
            .ToArray() ?? [];
    }

    private static ISeries[] CreateColumnSeries(IBaseStatisticChartViewModel chart)
    {
        return chart.Series?
            .Select(series => new ColumnSeries<double>
            {
                Name = series.Name,
                Values = series.Points.Select(point => point.Y).ToArray()
            })
            .Cast<ISeries>()
            .ToArray() ?? [];
    }

    private static ISeries[] CreateScatterSeries(IBaseStatisticChartViewModel chart)
    {
        return chart.Series?
            .Select(series => new ScatterSeries<LiveChartsCore.Defaults.ObservablePoint>
            {
                Name = series.Name,
                Values = series.Points
                    .Select(point => new LiveChartsCore.Defaults.ObservablePoint(point.X, point.Y))
                    .ToArray()
            })
            .Cast<ISeries>()
            .ToArray() ?? [];
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value))
        {
            return;
        }

        field = value;
        OnPropertyChanged(propertyName);
    }
}