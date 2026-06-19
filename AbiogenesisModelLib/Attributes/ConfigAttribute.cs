namespace AbiogenesisModel.Lib.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ConfigAttribute(string? directoryName = null) : Attribute
{
    public string? DirectoryName { get; } = directoryName;
}