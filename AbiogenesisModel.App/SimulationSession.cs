using AbiogenesisModel.Lib.Model;
using AbiogenesisModel.Lib.Model.Controllers;
using AbiogenesisModel.Lib.Model.DataTypes;
using AbiogenesisModel.Lib.Pipeline;
using AbiogenesisModel.Telemetry;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using AbiogenesisModel.Lib.DebugTools;
using AbiogenesisModel.Lib.Guard;

namespace AbiogenesisModel.App
{
    internal class SimulationSession
    {
        public enum State
        {
            Idle,
            Running,
            Paused,
            Stopped,
            Error
        }

        private readonly SimulationWorld? _simulationWorld;
        private readonly LifeCycle? _lifeCycle;
        private readonly SimulationStatisticsHub? _simulationStatisticsHub;

        private State _currentState;
        private long _currentTick;
        private int _reportEveryTicks = 1;

        public event PropertyChangedEventHandler? PropertyChanged;

        public SimulationSession()
        {
            try
            {
                var serviceProvider = InitServiceCollectionFromFiles();
                _simulationWorld = serviceProvider.GetRequiredService<SimulationWorldController>().Create();
                _lifeCycle = serviceProvider.GetRequiredService<LifeCycle>();
                _simulationStatisticsHub = serviceProvider.GetRequiredService<SimulationStatisticsHub>();

                CurrentState = State.Idle;
            }
            catch
            {
                CurrentState = State.Error;
            }
        }

        public State CurrentState
        {
            get => _currentState;
            private set
            {
                if (_currentState == value)
                {
                    return;
                }

                _currentState = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(State)));
            }
        }

        public long CurrentTick
        {
            get => _currentTick;
            private set
            {
                if (_currentTick == value)
                {
                    return;
                }

                _currentTick = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTick)));
            }
        }

        public int ReportEveryTicks
        {
            get => _reportEveryTicks;
            set
            {
                if (_reportEveryTicks == value)
                {
                    return;
                }

                Ensure.That(value).IsGreaterThan(0);
                _reportEveryTicks = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReportEveryTicks)));
            }
        }

        public async Task RunAsync(int tickCount, double? maxTicksPerSecond, IProgress<SimulationStatisticsFrame> progress, CancellationToken cancellationToken)
        {
            Ensure.That(CurrentState).IsInList([State.Idle, State.Stopped]);

            CurrentState = State.Running;

            var rateLimiter = maxTicksPerSecond is > 0 ? new TickRateLimiter(maxTicksPerSecond.Value) : null;
            var infinite = tickCount < 0;
            var executedTicks = 0;

            while (infinite || executedTicks < tickCount)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (new TimeMeasurer("ExecuteTick"))
                {
                    _lifeCycle!.ExecuteTick(_simulationWorld!);
                }

                CurrentTick++;
                executedTicks++;

                if (CurrentTick % _reportEveryTicks == 0)
                {
                    using (new TimeMeasurer("CollectStatistics"))
                    {
                        progress.Report(CollectStatistics());
                    }
                }

                if (rateLimiter is not null)
                {
                    await rateLimiter.WaitIfNeededAsync(cancellationToken);
                }
            }

            CurrentState = State.Stopped;
        }

        private SimulationStatisticsFrame CollectStatistics()
        {
            return _simulationStatisticsHub!.Collect(_simulationWorld!, _currentTick);
        }

        private static ServiceProvider InitServiceCollectionFromFiles()
        {
            var services = new ServiceCollection();
            services.RegisterGeneralConfigFromFile(string.Format(Constants.ConfigDirFormat, "general_test.yml"));
            services.RegisterConfigs();
            services.RegisterLibServices();
            services.RegisterTelemetryServices();

            return services.BuildServiceProvider();
        }
    }
}
