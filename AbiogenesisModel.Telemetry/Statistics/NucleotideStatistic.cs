using AbiogenesisModel.Lib.Model;
using AbiogenesisModel.Telemetry.Interfaces;

namespace AbiogenesisModel.Telemetry.Statistics;

public sealed record NucleotideStatistic(
    int TotalCount,
    int BondedCount,
    IReadOnlyDictionary<Nucleobase, int> PerNucleobaseCount)
    : ISimulationStatistic
{
}

