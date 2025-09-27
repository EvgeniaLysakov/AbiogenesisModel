using AbiogenesisModel.Core;
using ScottPlot;
using ScottPlot.TickGenerators;
using ScottPlot.WPF;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;

namespace AbiogenesisModel.Wpf
{
    public partial class MainWindow
    {
        private bool _stickToBottom = true;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = Vm;

            if (Vm.Feed is INotifyCollectionChanged ncc)
            {
                ncc.CollectionChanged += Feed_CollectionChanged;
            }

            FeedScroll.ScrollChanged += (_, e) =>
            {
                var nearBottom = e.VerticalOffset >= e.ExtentHeight - e.ViewportHeight - 1.0;
                _stickToBottom = nearBottom;
            };
        }

        public MainVm Vm { get; } = new();

        private void Feed_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (!_stickToBottom)
            {
                return;
            }

            if (e.Action is NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Reset)
            {
                // Ждём макет/отрисовку, иначе скролл может «не успеть»
                Dispatcher.InvokeAsync(() => FeedScroll.ScrollToEnd(), DispatcherPriority.Background);
            }
        }

        private void Plot_Loaded(object sender, RoutedEventArgs e)
        {
            var ctrl = (WpfPlot)sender;
            if (ctrl.DataContext is PlotItem p)
            {
                FillPlot(ctrl, p);
            }
        }

        private static void FillPlot(WpfPlot ctrl, PlotItem plotItem)
        {
            ctrl.Plot.Clear();
            ctrl.Plot.Title(plotItem.Title ?? string.Empty);
            ctrl.Plot.XLabel(plotItem.XTitle ?? string.Empty);
            ctrl.Plot.YLabel(plotItem.YTitle ?? string.Empty);

            switch (plotItem)
            {
                case LinePlotItem line:
                    FillPlotData(ctrl.Plot, line);
                    break;
                case BarPlotItem bar:
                    FillPlotData(ctrl.Plot, bar);
                    break;
            }

            ctrl.Refresh();
        }

        private static void FillPlotData(Plot plot, BarPlotItem barPlotItem)
        {
            var ticks = new List<Tick>();
            for (var i = 0; i < barPlotItem.Bars.Length; i++)
            {
                var (label, val) = barPlotItem.Bars[i];
                var barPlot = plot.Add.Bar(i + 1, val);
                foreach (var bar in barPlot.Bars)
                {
                    bar.Label = bar.Value.ToString(CultureInfo.InvariantCulture);
                }

                ticks.Add(new Tick(i + 1, label));
            }

            plot.Axes.Margins(bottom: 0);

            plot.Axes.Bottom.TickGenerator = new NumericManual(ticks.ToArray());
        }

        private static void FillPlotData(Plot plot, LinePlotItem linePlotItem)
        {
            plot.Add.Scatter(linePlotItem.X, linePlotItem.Y);
        }

        private void Loop_Click(object sender, RoutedEventArgs e)
        {
            Vm.LoopAndBars();
        }
    }
}