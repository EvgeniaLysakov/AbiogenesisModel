using AbiogenesisModel.Lib.Attributes;

namespace AbiogenesisModel.Lib.ConfigTypes.ControllerConfigTypes;

[Config("MoleculeController")]
public class MoleculeControllerConfig : ICloneable
{
    public object Clone()
    {
        return new MoleculeControllerConfig();
    }
}
