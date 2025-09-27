using System.Reflection;
using AbiogenesisModel.Lib.Steps;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AbiogenesisModel.Lib.Configurability
{
    public class ConfigurableObject
    {
        private static readonly ServiceProvider _serviceProvider;

        static ConfigurableObject()
        {
            var services = new ServiceCollection();
            services.AddSingleton<DynamicDependencyLib>();
            _serviceProvider = services.BuildServiceProvider();
        }

        protected ConfigurableObject()
            : this(LoadGeneralConfiguration())
        {
        }

        protected ConfigurableObject(IConfiguration config)
        {
            var type = GetType();
            config = config.GetChildren().FirstOrDefault(section => section.Key.Equals(type.Name, StringComparison.OrdinalIgnoreCase)) ?? config;
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var configurable = property.GetCustomAttributes(typeof(ConfigurableAttribute), inherit: false).OfType<ConfigurableAttribute>().FirstOrDefault();
                if (configurable == null)
                {
                    continue;
                }

                var propertyType = property.PropertyType;
                var propertyKey = configurable.Key;
                var value = propertyType.IsAssignableTo(typeof(IStep)) ?
                    CreateStepFromConfiguration(propertyType, propertyKey, config) :
                    config.GetValue(propertyType, propertyKey);
                property.SetValue(this, value ?? config.GetSection(propertyKey).Get(propertyType));
            }
        }

        public static string ConfigPath { get; } = "Config\\";

        public static string GeneralConfigFile { get; } = "general.yml";

        private static IStep CreateStepFromConfiguration(Type propertyType, string propertyKey, IConfiguration config)
        {
            var dependencyLib = _serviceProvider.GetRequiredService<DynamicDependencyLib>();
            var propertySection = config.GetSection(propertyKey);
            var implementationKey = propertySection.GetValue<string>("typeKey") ?? config.GetValue<string>(propertyKey);
            var implementationType = dependencyLib.ResolveDependency(implementationKey, propertyType);
            var constructors = implementationType.GetConstructors();
            IStep step;
            if (implementationType.IsSubclassOf(typeof(ConfigurableObject)) && constructors.Any(IsConfigurableConstructor))
            {
                var instanceConfigName = propertySection.GetValue("config", implementationType.Name + ".yml");
                var instanceConfig = new ConfigurationBuilder().AddConfiguration(config)
                    .AddYamlFile(ConfigPath + instanceConfigName, optional: true, reloadOnChange: false)
                    .Build();
                step = (IStep)ActivatorUtilities.CreateInstance(_serviceProvider, implementationType, instanceConfig);
            }
            else if (constructors.Any(ctr => !ctr.GetParameters().Any()))
            {
                step = (IStep)ActivatorUtilities.CreateInstance(_serviceProvider, implementationType);
            }
            else
            {
                throw new InvalidOperationException($"{implementationType.Name} is not configurable and doesn't have default constructor");
            }

            step.ValidateAndInit();
            return step;
        }

        private static bool IsConfigurableConstructor(ConstructorInfo ctr)
        {
            var parameters = ctr.GetParameters();
            return parameters.Length == 1 && typeof(IConfiguration).IsAssignableFrom(parameters[0].ParameterType);
        }

        private static IConfiguration LoadGeneralConfiguration()
        {
            var builder = new ConfigurationBuilder().AddYamlFile(ConfigPath + GeneralConfigFile, optional: false);
            return builder.Build();
        }
    }
}
