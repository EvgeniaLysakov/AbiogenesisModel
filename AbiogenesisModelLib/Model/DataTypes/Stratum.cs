using AbiogenesisModel.Lib.Attributes;

namespace AbiogenesisModel.Lib.Model;

[DataType]
public class Stratum
{
    internal Stratum(StratumPopulation currentPopulation, StratumPopulation sinkingPopulation)
    {
        CurrentPopulation = currentPopulation;
        SinkingPopulation = sinkingPopulation;
    }

    [Owned]
    public StratumPopulation CurrentPopulation { get; }

    [Owned]
    public StratumPopulation SinkingPopulation { get; }
}
