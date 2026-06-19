using AbiogenesisModel.Lib.Attributes;

namespace AbiogenesisModel.Lib.ConfigTypes.ControllerConfigTypes;

[Config("BondController")]
public class BondControllerConfig : ICloneable
{
    public object Clone()
    {
        return new BondControllerConfig();
    }
}