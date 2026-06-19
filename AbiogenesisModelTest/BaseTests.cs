using System.Reflection;
using AbiogenesisModel.Lib.Attributes;
using AbiogenesisModel.Lib.Pipeline;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace AbiogenesisModel.Test;

public class BaseTests
{
    protected ServiceProvider InitServiceCollectionFromFiles()
    {
        var services = new ServiceCollection();
        services.RegisterGeneralConfigFromFile(string.Format(Constants.ConfigDirFormat, "general_test.yml"));
        services.RegisterConfigs();
        services.RegisterLibServices();

        return services.BuildServiceProvider();
    }

    protected ServiceProvider InitServiceCollectionFromYamls(Dictionary<Type, string> configuredTypes, IReadOnlyList<Type> emptyTypes)
    {
        var services = new ServiceCollection();
        services.RegisterDefaultGeneralConfig();

        foreach (var pair in configuredTypes)
        {
            services.TryRegisterConfigFromYaml(pair.Key, pair.Value, Constants.DefaultKey).Should().BeTrue();
        }

        foreach (var type in emptyTypes)
        {
            services.TryRegisterEmptyConfig(type).Should().BeTrue();
        }

        services.RegisterLibServices();

        return services.BuildServiceProvider();
    }

    protected IReadOnlyList<Type> GetConfigTypes()
    {
        var assembly = Assembly.GetAssembly(typeof(Constants))!;
        return assembly.GetTypes().Where(t => t.GetCustomAttribute<ConfigAttribute>() != null).ToList();
    }

    protected IReadOnlyList<Type> GetServiceTypes()
    {
        var assembly = Assembly.GetAssembly(typeof(Constants))!;
        return assembly.GetTypes().Where(t => t.GetCustomAttribute<ServiceAttribute>() != null).ToList();
    }
}