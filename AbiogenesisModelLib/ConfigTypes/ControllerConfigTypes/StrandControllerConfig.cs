using AbiogenesisModel.Lib.Attributes;

namespace AbiogenesisModel.Lib.ConfigTypes.ControllerConfigTypes;

[Config("StrandController")]
public class StrandControllerConfig : ICloneable
{
    public object Clone()
    {
        return new StrandControllerConfig();
    }
}
