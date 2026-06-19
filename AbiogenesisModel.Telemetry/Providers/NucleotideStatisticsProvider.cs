using AbiogenesisModel.Lib.Attributes;
using AbiogenesisModel.Lib.DebugTools;
using AbiogenesisModel.Lib.Model;
using AbiogenesisModel.Lib.Model.DataTypes;
using AbiogenesisModel.Telemetry.Interfaces;
using AbiogenesisModel.Telemetry.Statistics;

namespace AbiogenesisModel.Telemetry.Providers;

[Service]
public class NucleotideStatisticsProvider : IStateStatisticsProvider
{
    public ISimulationStatistic Collect(SimulationWorld simulationWorld)
    {
        var perNucleobase = new Dictionary<Nucleobase, int>
        {
            { Nucleobase.A, 0 },
            { Nucleobase.U, 0 },
            { Nucleobase.C, 0 },
            { Nucleobase.G, 0 }
        };
        var total = 0;
        var bonded = 0;
        using (new TimeMeasurer($"{nameof(NucleotideStatisticsProvider)}.Collect"))
        {
            foreach (var nucleotide in simulationWorld.EnumerateNucleotides())
            {
                total++;
                perNucleobase[nucleotide.Base]++;
                if (nucleotide.Bond != null)
                {
                    bonded++;
                }
            }
        }

        return new NucleotideStatistic(total, bonded, perNucleobase);
    }
}