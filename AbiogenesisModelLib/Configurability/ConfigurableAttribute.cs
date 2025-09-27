namespace AbiogenesisModel.Lib.Configurability
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigurableAttribute(string key) : Attribute
    {
        public string Key { get; } = key;
    }
}
