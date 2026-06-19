using AbiogenesisModel.Lib.Attributes;

namespace AbiogenesisModel.Lib.ConfigTypes.ControllerConfigTypes;

[Config("ExternalEnvironmentController")]
public class ExternalEnvironmentControllerConfig : ICloneable
{
    public double InitialTemperature { get; set; }

    public object Clone()
    {
        return new ExternalEnvironmentControllerConfig() { InitialTemperature = InitialTemperature };
    }
}