using AbiogenesisModel.Lib.Attributes;

namespace AbiogenesisModel.Lib.Model;

[DataType]
public class ExternalEnvironment
{
    internal ExternalEnvironment(double temperature)
    {
        Temperature = temperature;
    }

    [Owned]
    public double Temperature { get; internal set; }
}