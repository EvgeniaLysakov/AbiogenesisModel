using AbiogenesisModel.Lib.Attributes;
using AbiogenesisModel.Lib.ConfigTypes.ControllerConfigTypes;
using AbiogenesisModel.Lib.Guard;
using AbiogenesisModel.Lib.Pipeline;

namespace AbiogenesisModel.Lib.Model.Controllers;

[Service]
public class BondController(IConfigFactory<BondControllerConfig> configFactory)
    : ConfigurableObject<BondControllerConfig>(configFactory)
{
    public Bond Create(Nucleotide nucleotide1, Nucleotide nucleotide2)
    {
        Ensure.That(nucleotide1).IsNotNull().IsNotBonded().IsNotSameAs(nucleotide2);
        Ensure.That(nucleotide2).IsNotNull().IsNotBonded();

        var bond = new Bond(nucleotide1, nucleotide2);
        nucleotide1.Bond = bond;
        nucleotide2.Bond = bond;
        return bond;
    }
}