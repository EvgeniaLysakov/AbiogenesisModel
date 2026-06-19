using AbiogenesisModel.Lib.ConfigTypes.ControllerConfigTypes;
using AbiogenesisModel.Lib.Events;
using AbiogenesisModel.Lib.Model;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace AbiogenesisModel.Test;

public class PipelineTests : BaseTests
{
    [Fact]
    public void FilesConfiguredServicesCreationTest()
    {
        var provider = InitServiceCollectionFromFiles();

        CreateServices(provider, []);
    }

    [Fact]
    public void YamlConfiguredServicesCreationTest()
    {
        var configuredTypes = new Dictionary<Type, string>
        {
            [typeof(SimulationWorldControllerConfig)] = "PondsNum: 1",
            [typeof(ExternalEnvironmentControllerConfig)] = "InitialTemperature: 20",
            [typeof(PondControllerConfig)] = "StrataNum: 5"
        };

        var emptyTypes = GetConfigTypes().Except(configuredTypes.Keys.ToArray()).ToArray();

        var provider = InitServiceCollectionFromYamls(configuredTypes, emptyTypes);

        CreateServices(provider, [typeof(SimpleThrowInEvent), typeof(LifeCycle)]);
    }

    private void CreateServices(ServiceProvider provider, Type[] exceptTypes)
    {
        var serviceTypes = GetServiceTypes();
        serviceTypes.Should().NotBeNullOrEmpty();

        using (new AssertionScope())
        {
            foreach (var serviceType in serviceTypes.Except(exceptTypes))
            {
                var service = provider.GetRequiredService(serviceType);
                service.Should().NotBeNull();
            }
        }
    }
}