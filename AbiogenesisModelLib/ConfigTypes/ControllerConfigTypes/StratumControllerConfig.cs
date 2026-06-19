using AbiogenesisModel.Lib.Attributes;

namespace AbiogenesisModel.Lib.ConfigTypes.ControllerConfigTypes;

[Config("StratumController")]
public class StratumControllerConfig : ICloneable
{
    public object Clone()
    {
        return new StratumControllerConfig();
    }
}
