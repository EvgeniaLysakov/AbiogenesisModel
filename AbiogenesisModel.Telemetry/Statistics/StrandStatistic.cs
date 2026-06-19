using AbiogenesisModel.Telemetry.Interfaces;

namespace AbiogenesisModel.Telemetry.Statistics;

public sealed record StrandStatistic(
    int TotalCount,
    IReadOnlyDictionary<int, int> PerLengthCount)
    : ISimulationStatistic
{
}