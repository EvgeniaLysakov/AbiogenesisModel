using AbiogenesisModel.Lib.Attributes;
using AbiogenesisModel.Lib.ConfigTypes.EventConfigTypes;
using AbiogenesisModel.Lib.EventContexts;
using AbiogenesisModel.Lib.Extensions;
using AbiogenesisModel.Lib.Guard;
using AbiogenesisModel.Lib.Interfaces;
using AbiogenesisModel.Lib.Pipeline;

namespace AbiogenesisModel.Lib.Events;

[Service]
public class SimpleThrowInEvent(IConfigFactory<SimpleThrowInEventConfig> configFactory)
    : ConfigurableObject<SimpleThrowInEventConfig>(configFactory), IPondEvent
{
    public bool TryExecute(PondEventContext context)
    {
        Ensure.That(context).IsNotNull();
        Ensure.That(context.Pond.Strata).IsNotEmpty();

        var random = new Random();
        var strandsCount = (int)random.NextNormal(Configuration.StrandsPerEventAverage, Configuration.StrandsPerEventVariance);

        if (strandsCount <= 0)
        {
            return false;
        }

        var strandsLengths = random.NextArray(strandsCount, Configuration.StrandLengthProbabilities);
        foreach (var strandLength in strandsLengths)
        {
            context.StratumPopulationController.Add(context.Pond.Strata[0].CurrentPopulation, random.NextArray(strandLength, Configuration.NucleobaseProbabilities));
        }

        return true;
    }
}