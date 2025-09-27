namespace AbiogenesisModel.Lib.Configurability
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ConfigurableStepAttribute(string key) : Attribute
    {
        public string Key { get; } = key;

        public bool IsDefault { get; set; }
    }
}
