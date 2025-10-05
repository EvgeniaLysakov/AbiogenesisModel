using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AbiogenesisModel.Lib.Pipeline;

public static class ServiceCollectionExtensions
{
    public static void RegisterServices(this IServiceCollection services)
    {
        services.RegisterServices(Assembly.GetExecutingAssembly());
    }

    public static void RegisterServices(this IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes().Where(t => t.GetCustomAttribute<ServiceAttribute>() != null);

        services.AddTransient(typeof(IKeyedFactory<>), typeof(KeyedFactory<>));
        services.AddTransient(typeof(IConfigFactory<>), typeof(ConfigFactory<>));

        foreach (var type in types)
        {
            var attr = type.GetCustomAttribute<ServiceAttribute>()!;
            var interfaces = type.GetInterfaces();
            foreach (var i in interfaces)
            {
                ServiceDescriptor descriptor;
                if (attr is NamedServiceAttribute namedAttr)
                {
                    descriptor = new ServiceDescriptor(i, namedAttr.Key, type, namedAttr.Lifetime);
                }
                else
                {
                    descriptor = new ServiceDescriptor(i, type, attr.Lifetime);
                }

                services.Add(descriptor);
            }

            if (!interfaces.Any())
            {
                var descriptor = new ServiceDescriptor(type, type, attr.Lifetime);
                services.Add(descriptor);
            }
        }
    }

    public static void RegisterDefaultGeneralConfig(this IServiceCollection services)
    {
        var path = "config\\general.yml";
        services.RegisterGeneralConfigFromFile(path);
    }

    public static void RegisterGeneralConfigFromFile(this IServiceCollection services, string path)
    {
        var yaml = File.ReadAllText(path);
        services.RegisterGeneralConfigFromYaml(yaml);
    }

    public static void RegisterGeneralConfigFromYaml(this IServiceCollection services, string yaml)
    {
        var deserializer = new DeserializerBuilder().WithNamingConvention(PascalCaseNamingConvention.Instance).Build();

        var configFactoryType = typeof(ConfigFactory);
        var instance = deserializer.Deserialize(yaml, configFactoryType);
        if (instance == null)
        {
            throw new InvalidOperationException($"Failed to deserialize general options from file 'general.yml'");
        }

        services.Add(new ServiceDescriptor(configFactoryType, instance));
    }

    public static void RegisterConfigs(this IServiceCollection services)
    {
        services.RegisterConfigs(Assembly.GetExecutingAssembly());
    }

    public static void RegisterConfigs(this IServiceCollection services, Assembly assembly)
    {
        var deserializer = new DeserializerBuilder().WithNamingConvention(PascalCaseNamingConvention.Instance).Build();

        var types = assembly.GetTypes().Where(t => t.GetCustomAttribute<ConfigAttribute>() != null);

        foreach (var type in types)
        {
            var attr = type.GetCustomAttribute<ConfigAttribute>()!;
            var configDir = $"config\\{attr.DirectoryName ?? type.Name}";
            if (!Directory.Exists(configDir))
            {
                throw new DirectoryNotFoundException($"The configuration directory '{configDir}' for options type '{type.FullName}' was not found.");
            }

            foreach (var path in Directory.EnumerateFiles(configDir, "*.yml"))
            {
                var yaml = File.ReadAllText(path);
                var instance = deserializer.Deserialize(yaml, type);

                if (instance == null)
                {
                    throw new InvalidOperationException($"Failed to deserialize options from file '{path}' to type '{type.FullName}'");
                }

                var key = Path.GetFileNameWithoutExtension(path);

                services.Add(new ServiceDescriptor(type, key, instance));
            }
        }
    }
}