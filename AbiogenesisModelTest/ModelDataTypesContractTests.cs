using AbiogenesisModel.Lib.Attributes;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Reflection;

namespace AbiogenesisModel.Test;

public class ModelDataTypesContractTests
{
    private static readonly Type[] ClassificationAttributes =
    [
        typeof(OwnedAttribute),
        typeof(KnowsAttribute),
        typeof(RuntimeAttribute)
    ];

    [Fact]
    public void PropertyAttributionTest()
    {
        var modelDataTypes = GetModelDataTypes();

        using (new AssertionScope())
        {
            foreach (var type in modelDataTypes)
            {
                var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                foreach (var property in properties)
                {
                    var attributes = GetClassificationAttributes(property);

                    attributes.Should().HaveCount(1, $"Property '{type.FullName}.{property.Name}' must have exactly one classification attribute");
                }
            }
        }
    }

    [Fact]
    public void NoPublicConstructorsTest()
    {
        var modelDataTypes = GetModelDataTypes();

        using (new AssertionScope())
        {
            foreach (var type in modelDataTypes)
            {
                var constructors = type.GetConstructors(BindingFlags.Public);

                constructors.Should().BeEmpty($"Type '{type.FullName}' shouldn't have public constructors");
            }
        }
    }

    [Fact]
    public void NoPublicSettersTest()
    {
        var modelDataTypes = GetModelDataTypes();

        using (new AssertionScope())
        {
            foreach (var type in modelDataTypes)
            {
                var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                foreach (var property in properties)
                {
                    property.SetMethod?.IsPublic.Should().NotBe(true, $"Property '{type.FullName}.{property.Name}' shouldn't have a public setter");
                }
            }
        }
    }

    private static Type[] GetModelDataTypes()
    {
        return typeof(DataTypeAttribute).Assembly
            .GetTypes()
            .Where(IsModelDataTypeClass)
            .OrderBy(t => t.FullName, StringComparer.Ordinal)
            .ToArray();
    }

    private static Type[] GetClassificationAttributes(PropertyInfo property)
    {
        return property
            .GetCustomAttributes(inherit: true)
            .Select(a => a.GetType())
            .Where(IsClassificationAttribute)
            .ToArray();
    }

    private static bool IsModelDataTypeClass(Type type)
    {
        return type.GetCustomAttribute<DataTypeAttribute>() != null;
    }

    private static bool IsClassificationAttribute(Type attributeType)
    {
        return ClassificationAttributes.Contains(attributeType);
    }

    [Fact]
    public void ModelDataTypesTreeTest()
    {
        var modelDataTypes = GetModelDataTypes();
        var ownedEdges = BuildOwnedEdges(modelDataTypes);

        using (new AssertionScope())
        {
            AssertNoMultipleParents(ownedEdges);
            AssertNoCycles(modelDataTypes, ownedEdges);
        }
    }

    private static IReadOnlyDictionary<Type, IReadOnlyList<Type>> BuildOwnedEdges(IReadOnlyList<Type> dataClassTypes)
    {
        var dataClassTypeSet = new HashSet<Type>(dataClassTypes);
        var result = new Dictionary<Type, IReadOnlyList<Type>>();

        foreach (var ownerType in dataClassTypes)
        {
            var childTypes = ownerType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(property => property.GetIndexParameters().Length == 0)
                .Where(HasOwnedAttribute)
                .SelectMany(property => GetOwnedDataClassTargets(property, dataClassTypeSet))
                .Distinct()
                .OrderBy(type => type.FullName, StringComparer.Ordinal)
                .ToArray();

            result.Add(ownerType, childTypes);
        }

        return result;
    }

    private static bool HasOwnedAttribute(PropertyInfo property)
    {
        return property.IsDefined(typeof(OwnedAttribute), inherit: true);
    }

    private static IReadOnlyList<Type> GetOwnedDataClassTargets(
        PropertyInfo property,
        ISet<Type> dataClassTypes)
    {
        var propertyType = property.PropertyType;

        var directTarget = TryGetDirectDataClassType(propertyType, dataClassTypes);
        if (directTarget is not null)
        {
            return [directTarget];
        }

        var elementType = TryGetCollectionElementType(propertyType);
        if (elementType is not null && dataClassTypes.Contains(elementType))
        {
            return [elementType];
        }

        return Array.Empty<Type>();
    }

    private static Type? TryGetDirectDataClassType(Type type, ISet<Type> dataClassTypes)
    {
        if (dataClassTypes.Contains(type))
        {
            return type;
        }

        var nullableUnderlyingType = Nullable.GetUnderlyingType(type);
        if (nullableUnderlyingType is not null && dataClassTypes.Contains(nullableUnderlyingType))
        {
            return nullableUnderlyingType;
        }

        return null;
    }

    private static Type? TryGetCollectionElementType(Type type)
    {
        if (type.IsArray)
        {
            return type.GetElementType();
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            return type.GetGenericArguments()[0];
        }

        var enumerableInterface = type
            .GetInterfaces()
            .FirstOrDefault(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

        return enumerableInterface?.GetGenericArguments()[0];
    }

    private static void AssertNoMultipleParents(IReadOnlyDictionary<Type, IReadOnlyList<Type>> ownedEdges)
    {
        var parentMap = new Dictionary<Type, List<Type>>();

        foreach (var (parent, children) in ownedEdges)
        {
            foreach (var child in children)
            {
                if (!parentMap.TryGetValue(child, out var parents))
                {
                    parents = [];
                    parentMap.Add(child, parents);
                }

                parents.Add(parent);
            }
        }

        foreach (var (child, parents) in parentMap.OrderBy(x => x.Key.FullName, StringComparer.Ordinal))
        {
            parents.Should().HaveCountLessThanOrEqualTo(1, $"type '{child.FullName}' must have at most one owner, but is owned by: {string.Join(", ", parents.Select(p => p.FullName))}");
        }
    }

    private static void AssertNoCycles(
        IReadOnlyList<Type> dataClassTypes,
        IReadOnlyDictionary<Type, IReadOnlyList<Type>> ownedEdges)
    {
        var stateByType = dataClassTypes.ToDictionary(type => type, _ => VisitState.NotVisited);

        foreach (var type in dataClassTypes)
        {
            if (stateByType[type] == VisitState.NotVisited)
            {
                DepthFirstSearch(type, ownedEdges, stateByType, new Stack<Type>());
            }
        }
    }

    private static void DepthFirstSearch(
        Type current,
        IReadOnlyDictionary<Type, IReadOnlyList<Type>> ownedEdges,
        IDictionary<Type, VisitState> stateByType,
        Stack<Type> path)
    {
        stateByType[current] = VisitState.Visiting;
        path.Push(current);

        foreach (var next in ownedEdges[current])
        {
            var nextState = stateByType[next];
            nextState.Should().NotBe(VisitState.Visiting, "Cycle detected in Owned type graph: {0}", BuildCycleDescription(path, next));

            if (nextState == VisitState.NotVisited)
            {
                DepthFirstSearch(next, ownedEdges, stateByType, path);
            }
        }

        path.Pop();
        stateByType[current] = VisitState.Visited;
    }

    private static string BuildCycleDescription(IEnumerable<Type> path, Type repeatedNode)
    {
        var reversedPath = path.Reverse().ToList();
        var cycleStartIndex = reversedPath.FindIndex(type => type == repeatedNode);

        var cycleNodes = reversedPath
            .Skip(cycleStartIndex)
            .Concat([repeatedNode])
            .Select(type => type.FullName)
            .ToArray();

        return string.Join(" -> ", cycleNodes);
    }

    private enum VisitState
    {
        NotVisited,
        Visiting,
        Visited,
    }
}