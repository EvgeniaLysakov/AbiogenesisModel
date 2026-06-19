using AbiogenesisModel.Lib.Attributes;

namespace AbiogenesisModel.Lib.ConfigTypes.ControllerConfigTypes;

[Config("StratumPopulationController")]
public class StratumPopulationControllerConfig : ICloneable
{
    public object Clone()
    {
        return new StratumPopulationControllerConfig();
    }
}
