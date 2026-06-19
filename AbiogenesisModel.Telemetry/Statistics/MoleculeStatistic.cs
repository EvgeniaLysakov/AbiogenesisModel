using AbiogenesisModel.Telemetry.Interfaces;

namespace AbiogenesisModel.Telemetry.Statistics;

public sealed record MoleculeStatistic(
    int TotalCount,
    IReadOnlyDictionary<int, int> PerStrandNumCount)
    : ISimulationStatistic
{
}
