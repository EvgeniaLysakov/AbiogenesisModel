using AbiogenesisModel.Lib.Extensions;
using System.Collections.Concurrent;
using System.Reflection;

namespace AbiogenesisModel.Lib.Configurability
{
    public sealed class DynamicDependencyLib
    {
        private readonly Func<IEnumerable<Assembly>> _assembliesProvider;
        private readonly ConcurrentDictionary<Type, Lazy<TypeMap>> _mapCache = new();

        private int _version;

        public DynamicDependencyLib(Func<IEnumerable<Assembly>>? assembliesProvider = null)
        {
            _assembliesProvider = assembliesProvider ?? (() => AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic));
            AppDomain.CurrentDomain.AssemblyLoad += (_, _) => InvalidateAll();
        }

        public Type ResolveDependency(string? key, Type type)
        {
            var map = GetOrBuildMap(type);
            Type? implementationType;
            if (key == null)
            {
                implementationType = map.Default;
                if (implementationType == null)
                {
                    throw new InvalidOperationException($"Default implementation is not found for {type.FullName}. Available: {map.All.Keys.Join()}");
                }
            }
            else if (!map.All.TryGetValue(key, out implementationType))
            {
                throw new InvalidOperationException($"Implementation '{key}' is not found for {type.FullName}. Available: {map.All.Keys.Join()}");
            }

            return implementationType;
        }

        public void InvalidateAll()
        {
            Interlocked.Increment(ref _version);
            _mapCache.Clear();
        }

        private TypeMap GetOrBuildMap(Type type)
        {
            if (!type.IsInterface)
            {
                throw new ArgumentException($"{type.FullName} is not an interface");
            }

            var versionAtStart = Volatile.Read(ref _version);
            var lazy = _mapCache.GetOrAdd(type, _ => new Lazy<TypeMap>(() => BuildMap(type), LazyThreadSafetyMode.ExecutionAndPublication));

            var map = lazy.Value;

            if (versionAtStart == Volatile.Read(ref _version))
            {
                return map;
            }

            _mapCache.TryRemove(type, out _);
            return GetOrBuildMap(type);
        }

        private TypeMap BuildMap(Type mappingType)
        {
            var assemblies = _assembliesProvider();

            var types = assemblies.SelectMany(asm => asm.GetAssignableTypes(mappingType)).Distinct();

            var dict = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            Type? defaultType = null;

            foreach (var t in types.ExcludeNull())
            {
                var attribute = t.GetCustomAttribute<ConfigurableStepAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                var key = attribute.Key;

                if (dict.TryGetValue(key, out var existing))
                {
                    throw new InvalidOperationException($"for the interface {mappingType.Name} the key {key} has more than one implementation: {t.Name}, {existing.Name}");
                }

                dict[key] = t;

                if (!attribute.IsDefault)
                {
                    continue;
                }

                if (defaultType != null)
                {
                    throw new InvalidOperationException($"for the interface {mappingType.Name} there are more than one default implementation: {t.Name}, {defaultType.Name}");
                }

                defaultType = t;
            }

            return new TypeMap(dict, defaultType);
        }

        private class TypeMap(Dictionary<string, Type> all, Type? defaultType)
        {
            public Dictionary<string, Type> All { get; } = all;

            public Type? Default { get; } = defaultType;
        }
    }
}
