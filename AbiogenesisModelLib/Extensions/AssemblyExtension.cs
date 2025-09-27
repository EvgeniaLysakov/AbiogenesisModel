using System.Reflection;

namespace AbiogenesisModel.Lib.Extensions
{
    public static class AssemblyExtension
    {
        public static IEnumerable<Type?> GetAssignableTypes(this Assembly a, Type baseType)
        {
            IEnumerable<Type?> types;
            try
            {
                types = a.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types.ExcludeNull().ToArrayOrCast();
            }

            return types.Where(t => t is { IsClass: true, IsAbstract: false } && baseType.IsAssignableFrom(t));
        }
    }
}
