using AbiogenesisModel.Lib.EventContexts;
using AbiogenesisModel.Lib.Events;
using AbiogenesisModel.Lib.Model.Controllers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace AbiogenesisModel.Test;

public class EventsTests : BaseTests
{
    [Fact]
    public void SimpleThrowInEventTest()
    {
        var provider = InitServiceCollectionFromFiles();
        var controller = provider.GetRequiredService<SimulationWorldController>();
        var world = controller.Create();

        var contextFactory = provider.GetRequiredService<PondEventContextFactory>();
        contextFactory.Should().NotBeNull();
        var context = contextFactory.Create(world.ExternalEnvironment, world.Ponds[0]);

        var throwInEvent = provider.GetRequiredService<SimpleThrowInEvent>();
        throwInEvent.Should().NotBeNull();

        var attempts = 0;
        while (!throwInEvent.TryExecute(context))
        {
            attempts++;
            attempts.Should().BeLessThan(5);
        }

        world.Ponds[0].Strata[0].CurrentPopulation.Molecules.Count.Should().BeGreaterThan(0);
    }
}