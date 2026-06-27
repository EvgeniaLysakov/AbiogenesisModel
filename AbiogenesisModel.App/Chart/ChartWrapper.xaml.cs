using LiveChartsCore.Measure;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace AbiogenesisModel.App.Chart;

/// <summary>
/// Interaction logic for ChartWrapper.xaml
/// </summary>
public partial class ChartWrapper : INotifyPropertyChanged
{
    private LegendPosition _legendPosition = LegendPosition.Hidden;

    public ChartWrapper()
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Title => ChartViewModel?.Title ?? string.Empty;

    public LegendPosition LegendPosition
    {
        get => _legendPosition;
        set => SetField(ref _legendPosition, value);
    }

    public LiveChartAdapterViewModel LiveChartAdapterViewModel { get; } = new();

    public static readonly DependencyProperty ChartViewModelProperty =
        DependencyProperty.Register(
            nameof(ChartViewModel),
            typeof(IBaseStatisticChartViewModel),
            typeof(ChartWrapper),
            new PropertyMetadata(null, OnChartViewModelChanged));

    public IBaseStatisticChartViewModel? ChartViewModel
    {
        get => (IBaseStatisticChartViewModel?)GetValue(ChartViewModelProperty);
        set => SetValue(ChartViewModelProperty, value);
    }

    private static void OnChartViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctrl = (ChartWrapper)d;
        if (e.OldValue is IBaseStatisticChartViewModel oldVm)
        {
            oldVm.PropertyChanged -= ctrl.ChartViewModel_PropertyChanged;
        }

        if (e.NewValue is IBaseStatisticChartViewModel newVm)
        {
            newVm.PropertyChanged += ctrl.ChartViewModel_PropertyChanged;
            ctrl.LiveChartAdapterViewModel.Update(newVm);
        }

        ctrl.OnPropertyChanged(nameof(Title));
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

    private void ChartViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(IBaseStatisticChartViewModel.Title):
                OnPropertyChanged(nameof(Title));
                break;
            case nameof(IBaseStatisticChartViewModel.Kind):
            case nameof(IBaseStatisticChartViewModel.XAxis):
            case nameof(IBaseStatisticChartViewModel.YAxis):
            case nameof(IBaseStatisticChartViewModel.Series):
                LiveChartAdapterViewModel.Update(ChartViewModel);
                OnPropertyChanged(nameof(LiveChartAdapterViewModel));
                break;
        }
    }
}