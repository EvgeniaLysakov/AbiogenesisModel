using AbiogenesisModel.App.Chart;
using AbiogenesisModel.App.Statistics;
using AbiogenesisModel.Telemetry;
using AbiogenesisModel.Telemetry.Statistics;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AbiogenesisModel.App;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private readonly IStatisticsHistoryStore _historyStore;

    private CancellationTokenSource? _cancellationTokenSource;

    private SimulationSession? _simulationSession;

    private string _status = string.Empty;

    private bool _loadCommandEnabled;
    private bool _runCommandEnabled;
    private bool _pauseCommandEnabled;
    private bool _stopCommandEnabled;
    private bool _saveSnapshotCommandEnabled;

    private long _currentTick;
    private double _ticksPerSecond;
    private string _elapsed = "00:00:00";
    private double _progress;

    private int _molecules;
    private int _strands;
    private int _nucleotides;
    private int _eventsPerSecond;

    public event PropertyChangedEventHandler? PropertyChanged;

    public MainViewModel()
    {
        _historyStore = new StatisticsHistoryStore();

        ConfigurationFile = "config.yml";
        OutputDirectory = "runs/run-001";

        LoadCommand = new RelayCommand(Load);
        RunCommand = new AsyncRelayCommand(RunAsync);
        PauseCommand = new RelayCommand(Pause);
        StopCommand = new RelayCommand(Stop);
        SaveSnapshotCommand = new RelayCommand(SaveSnapshot);
        ShowConfigurationFilesCommand = new RelayCommand(ShowConfigurationFiles);

        EventCounters.Add(new EventCounterViewModel("Hybridization", 0, 0, 0));
        EventCounters.Add(new EventCounterViewModel("Ligation", 0, 0, 0));

        LogMessages.Add("Application started");
        LogMessages.Add("Waiting for configuration");

        UpdateStatus();
        UpdateCommandsState();
        UpdateCharts();
    }

    public string ConfigurationFile { get; set; }
    public string OutputDirectory { get; set; }
    public string MaxTicks { get; set; } = "1000";
    public string ReportEveryTicks { get; set; } = "10";

    public string Status
    {
        get => _status;
        private set => SetField(ref _status, value);
    }

    public long CurrentTick
    {
        get => _currentTick;
        private set => SetField(ref _currentTick, value);
    }

    public double TicksPerSecond
    {
        get => _ticksPerSecond;
        private set => SetField(ref _ticksPerSecond, value);
    }

    public string Elapsed
    {
        get => _elapsed;
        private set => SetField(ref _elapsed, value);
    }

    public double Progress
    {
        get => _progress;
        private set => SetField(ref _progress, value);
    }

    public int Molecules
    {
        get => _molecules;
        private set => SetField(ref _molecules, value);
    }

    public int Strands
    {
        get => _strands;
        private set => SetField(ref _strands, value);
    }

    public int Nucleotides
    {
        get => _nucleotides;
        private set => SetField(ref _nucleotides, value);
    }

    public int EventsPerSecond
    {
        get => _eventsPerSecond;
        private set => SetField(ref _eventsPerSecond, value);
    }

    public ObservableCollection<EventCounterViewModel> EventCounters { get; } = [];

    public ObservableCollection<string> LogMessages { get; } = [];

    public IBaseStatisticChartViewModel StrandLengthDistributionChart { get; } = new StrandLengthDistributionViewModel();

    public IBaseStatisticChartViewModel StrandTotalChart { get; } = new StrandTotalOverTimeViewModel();

    public bool LoadCommandEnabled
    {
        get => _loadCommandEnabled;
        private set => SetField(ref _loadCommandEnabled, value);
    }

    public bool RunCommandEnabled
    {
        get => _runCommandEnabled;
        private set => SetField(ref _runCommandEnabled, value);
    }

    public bool PauseCommandEnabled
    {
        get => _pauseCommandEnabled;
        private set => SetField(ref _pauseCommandEnabled, value);
    }

    public bool StopCommandEnabled
    {
        get => _stopCommandEnabled;
        private set => SetField(ref _stopCommandEnabled, value);
    }

    public bool SaveSnapshotCommandEnabled
    {
        get => _saveSnapshotCommandEnabled;
        private set => SetField(ref _saveSnapshotCommandEnabled, value);
    }

    public ICommand LoadCommand { get; }
    public ICommand RunCommand { get; }
    public ICommand PauseCommand { get; }
    public ICommand StopCommand { get; }
    public ICommand SaveSnapshotCommand { get; }
    public ICommand ShowConfigurationFilesCommand { get; }

    private void Load()
    {
        if (_simulationSession != null)
        {
            _simulationSession.PropertyChanged -= OnSessionPropertyChanged;
        }

        _simulationSession = new SimulationSession();
        _simulationSession.PropertyChanged += OnSessionPropertyChanged;
        LogMessages.Add("Configuration loaded");

        UpdateStatus();
        UpdateCommandsState();
        UpdateCharts();
    }

    private async Task RunAsync()
    {
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();

        var progress = new Progress<SimulationStatisticsFrame>(ApplyFrame);

        try
        {
            if (!int.TryParse(MaxTicks, out var maxTicks))
            {
                LogMessages.Add("Invalid MaxTicks value");
                return;
            }

            if (!int.TryParse(ReportEveryTicks, out var reportEveryTicks))
            {
                LogMessages.Add("Invalid ReportEveryTicks value");
                return;
            }

            _simulationSession!.ReportEveryTicks = reportEveryTicks;

            const int maxTicksPerSecond = 10;

            LogMessages.Add("Simulation started");
            await _simulationSession!.RunAsync(maxTicks, maxTicksPerSecond, progress, _cancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void OnSessionPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(SimulationSession.CurrentState):
                UpdateStatus();
                UpdateCommandsState();
                break;
            case nameof(SimulationSession.CurrentTick):
                CurrentTick = _simulationSession!.CurrentTick;
                break;
        }
    }

    private void ApplyFrame(SimulationStatisticsFrame frame)
    {
        _historyStore.AddFrame(frame);

        CurrentTick = frame.Tick;

        if (frame.TryGet<StrandStatistic>(out var strands))
        {
            Strands = strands.TotalCount;
        }

        if (frame.TryGet<MoleculeStatistic>(out var molecules))
        {
            Molecules = molecules.TotalCount;
        }

        if (frame.TryGet<NucleotideStatistic>(out var nucleotides))
        {
            Nucleotides = nucleotides.TotalCount;
        }

        UpdateCharts();
    }

    private void Pause()
    {
    }

    private void Stop()
    {
        _cancellationTokenSource?.Cancel();
    }

    private void SaveSnapshot()
    {
    }

    private void ShowConfigurationFiles()
    {
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

    private void UpdateStatus()
    {
        Status = _simulationSession?.CurrentState.ToString() ?? "Not loaded";
    }

    private void UpdateCommandsState()
    {
        LoadCommandEnabled = _simulationSession == null || _simulationSession.CurrentState == SimulationSession.State.Idle || _simulationSession.CurrentState == SimulationSession.State.Stopped;
        RunCommandEnabled = _simulationSession is { CurrentState: SimulationSession.State.Idle or SimulationSession.State.Paused };
        PauseCommandEnabled = _simulationSession is { CurrentState: SimulationSession.State.Running };
        StopCommandEnabled = _simulationSession is { CurrentState: SimulationSession.State.Running or SimulationSession.State.Paused };
        SaveSnapshotCommandEnabled = _simulationSession is { CurrentState: SimulationSession.State.Paused };
    }

    private void UpdateCharts()
    {
        var strandSeries = _historyStore.GetSeries<StrandStatistic>();

        if (strandSeries.Any())
        {
            StrandLengthDistributionChart.TryToUpdate(strandSeries);
            StrandTotalChart.TryToUpdate(strandSeries);
        }
        else
        {
            StrandLengthDistributionChart.Clear();
            StrandTotalChart.Clear();
        }
    }
}