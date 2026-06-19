using AbiogenesisModel.Lib.Attributes;
using AbiogenesisModel.Lib.Extensions;
using AbiogenesisModel.Lib.Model.DataTypes;
using AbiogenesisModel.Telemetry.Interfaces;

namespace AbiogenesisModel.Telemetry;

[Service]
public class SimulationStatisticsHub(IEnumerable<IStateStatisticsProvider>? stateStatisticsProviders, IEnumerable<ICachedStatisticsProvider>? cachedStatisticsProviders)
{
    private readonly IEnumerable<IStateStatisticsProvider> _stateStatisticsProviders = stateStatisticsProviders ?? [];
    private readonly IEnumerable<ICachedStatisticsProvider> _cachedStatisticsProviders = cachedStatisticsProviders ?? [];

    public SimulationStatisticsFrame Collect(SimulationWorld world, long tick)
    {
        return new SimulationStatisticsFrame(tick, _stateStatisticsProviders.Select(provider => provider.Collect(world)).Concat(_cachedStatisticsProviders.Select(provider => provider.Flush())).ExcludeNull());
    }
}