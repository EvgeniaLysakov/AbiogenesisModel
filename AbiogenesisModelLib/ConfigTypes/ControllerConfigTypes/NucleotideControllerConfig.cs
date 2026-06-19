using AbiogenesisModel.Lib.Attributes;

namespace AbiogenesisModel.Lib.ConfigTypes.ControllerConfigTypes;

[Config("NucleotideController")]
public class NucleotideControllerConfig : ICloneable
{
    public object Clone()
    {
        return new NucleotideControllerConfig();
    }
}
