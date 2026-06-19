using AbiogenesisModel.Lib.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AbiogenesisModel.Lib.Pipeline;

public static class ServiceCollectionExtensions
{
    private static readonly IDeserializer Deserializer = new DeserializerBuilder().WithNamingConvention(PascalCaseNamingConvention.Instance).Build();

    public static void RegisterLibServices(this IServiceCollection services)
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
            var serviceTypes = type.GetInterfaces().Concat([type]);
            foreach (var serviceType in serviceTypes)
            {
                ServiceDescriptor descriptor;
                if (attr is NamedServiceAttribute namedAttr)
                {
                    descriptor = new ServiceDescriptor(serviceType, namedAttr.Key, type, namedAttr.Lifetime);
                }
                else
                {
                    descriptor = new ServiceDescriptor(serviceType, type, attr.Lifetime);
                }

                services.Add(descriptor);
            }
        }
    }

    public static void RegisterGeneralConfigFromFile(this IServiceCollection services)
    {
        services.RegisterGeneralConfigFromFile(Constants.GeneralConfigPath);
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

        services.Add(new ServiceDescriptor(configFactoryType, instance!));
    }

    public static void RegisterDefaultGeneralConfig(this IServiceCollection services)
    {
        var configFactoryType = typeof(ConfigFactory);
        services.Add(new ServiceDescriptor(configFactoryType, new ConfigFactory() { ConfigKeys = new Dictionary<string, string>() }));
    }

    public static void RegisterConfigs(this IServiceCollection services)
    {
        services.RegisterConfigs(Assembly.GetExecutingAssembly());
    }

    public static void RegisterConfigs(this IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes().Where(t => t.GetCustomAttribute<ConfigAttribute>() != null);

        foreach (var type in types)
        {
            var attr = type.GetCustomAttribute<ConfigAttribute>()!;
            var configDir = string.Format(Constants.ConfigDirFormat, attr.DirectoryName ?? type.Name);
            if (!Directory.Exists(configDir))
            {
                services.TryRegisterEmptyConfig(type);
                continue;
            }

            foreach (var path in Directory.EnumerateFiles(configDir, Constants.YmlFilesPattern))
            {
                var yaml = File.ReadAllText(path);
                var key = Path.GetFileNameWithoutExtension(path);

                services.TryRegisterConfigFromYaml(type, yaml, key);
            }
        }
    }

    public static bool TryRegisterConfigFromYaml(this IServiceCollection services, Type type, string yaml, string key)
    {
        var instance = Deserializer.Deserialize(yaml, type);

        if (instance == null)
        {
            return false;
        }

        services.Add(new ServiceDescriptor(type, key, instance));
        return true;

    }

    public static bool TryRegisterEmptyConfig(this IServiceCollection services, Type type)
    {
        var instance = Activator.CreateInstance(type);

        if (instance == null)
        {
            return false;
        }

        services.Add(new ServiceDescriptor(type, Constants.DefaultKey, instance));
        return true;

    }
}