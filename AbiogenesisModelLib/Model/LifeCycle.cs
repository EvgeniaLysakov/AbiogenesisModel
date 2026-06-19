using AbiogenesisModel.Lib.Attributes;
using AbiogenesisModel.Lib.EventContexts;
using AbiogenesisModel.Lib.Extensions;
using AbiogenesisModel.Lib.Interfaces;
using AbiogenesisModel.Lib.Model.DataTypes;

namespace AbiogenesisModel.Lib.Model;

[Service]
public class LifeCycle(
    IEnumerable<IEnvironmentEvent>? environmentEvents,
    IEnumerable<IPondEvent>? pondEvents,
    IEnumerable<IStratumEvent>? stratumEvents,
    EnvironmentEventContextFactory environmentEventContextFactory,
    PondEventContextFactory pondEventContextFactory,
    StratumEventContextFactory stratumEventContextFactory)
{
    private readonly IEnvironmentEvent[] _environmentEvents = environmentEvents?.ToArray() ?? [];
    private readonly IPondEvent[] _pondEvents = pondEvents?.ToArray() ?? [];
    private readonly IStratumEvent[] _stratumEvents = stratumEvents?.ToArray() ?? [];

    public void ExecuteTick(SimulationWorld simulationWorld)
    {
        ExecuteEnvironmentEvents(environmentEventContextFactory.Create(simulationWorld.ExternalEnvironment));

        foreach (var pond in simulationWorld.Ponds)
        {
            ExecutePondEvents(pondEventContextFactory.Create(simulationWorld.ExternalEnvironment, pond));

            foreach (var stratum in pond.Strata)
            {
                ExecuteStratumEvents(stratumEventContextFactory.Create(simulationWorld.ExternalEnvironment, stratum));
            }
        }
    }

    private void ExecuteEnvironmentEvents(EnvironmentEventContext context)
    {
        foreach (var environmentEvent in _environmentEvents)
        {
            _ = environmentEvent.TryExecute(context);
        }
    }

    private void ExecutePondEvents(PondEventContext context)
    {
        foreach (var pondEvent in _pondEvents)
        {
            _ = pondEvent.TryExecute(context);
        }
    }

    private void ExecuteStratumEvents(StratumEventContext context)
    {
        var totalVelocity = 0;
        var stratumEventProbabilities = new Dictionary<IStratumEvent, double>();
        foreach (var stratumEvent in _stratumEvents)
        {
            var velocity = stratumEvent.GetVelocity(context);
            totalVelocity += velocity;
            stratumEventProbabilities[stratumEvent] = velocity;
        }

        if (totalVelocity == 0)
        {
            return;
        }

        foreach (var stratumEvent in _stratumEvents)
        {
            stratumEventProbabilities[stratumEvent] /= totalVelocity;
        }

        var random = new Random();
        var plannedEvents = random.NextArray(totalVelocity, stratumEventProbabilities);

        foreach (var stratumEvent in plannedEvents)
        {
            _ = stratumEvent.TryExecute(context);
        }
    }
}