using AbiogenesisModel.Lib.Attributes;
using AbiogenesisModel.Lib.DebugTools;
using AbiogenesisModel.Lib.Model.DataTypes;
using AbiogenesisModel.Telemetry.Interfaces;
using AbiogenesisModel.Telemetry.Statistics;

namespace AbiogenesisModel.Telemetry.Providers;

[Service]
public class MoleculeStatisticsProvider : IStateStatisticsProvider
{
    public ISimulationStatistic Collect(SimulationWorld simulationWorld)
    {
        var perStrandCount = new Dictionary<int, int>();
        var total = 0;
        using (new TimeMeasurer($"{nameof(MoleculeStatisticsProvider)}.Collect"))
        {
            foreach (var molecule in simulationWorld.EnumerateMolecules())
            {
                total++;
                var length = molecule.Strands.Count;
                if (!perStrandCount.TryAdd(length, 1))
                {
                    perStrandCount[length]++;
                }
            }
        }

        return new MoleculeStatistic(total, perStrandCount);
    }
}