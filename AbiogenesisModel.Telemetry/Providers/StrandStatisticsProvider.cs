using AbiogenesisModel.Lib.Attributes;
using AbiogenesisModel.Lib.DebugTools;
using AbiogenesisModel.Lib.Model.DataTypes;
using AbiogenesisModel.Telemetry.Interfaces;
using AbiogenesisModel.Telemetry.Statistics;

namespace AbiogenesisModel.Telemetry.Providers;

[Service]
public class StrandStatisticsProvider : IStateStatisticsProvider
{
    public ISimulationStatistic Collect(SimulationWorld simulationWorld)
    {
        var perLength = new Dictionary<int, int>();
        var total = 0;
        using (new TimeMeasurer($"{nameof(StrandStatisticsProvider)}.Collect"))
        {
            foreach (var strand in simulationWorld.EnumerateStrands())
            {
                total++;
                var length = strand.Nucleotides.Length;
                if (!perLength.TryAdd(length, 1))
                {
                    perLength[length]++;
                }
            }
        }

        return new StrandStatistic(total, perLength);
    }
}