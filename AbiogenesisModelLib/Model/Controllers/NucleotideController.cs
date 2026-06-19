using AbiogenesisModel.Lib.Attributes;
using AbiogenesisModel.Lib.ConfigTypes.ControllerConfigTypes;
using AbiogenesisModel.Lib.Pipeline;

namespace AbiogenesisModel.Lib.Model.Controllers;

[Service]
public class NucleotideController(IConfigFactory<NucleotideControllerConfig> configFactory)
    : ConfigurableObject<NucleotideControllerConfig>(configFactory)
{
    public Nucleotide Create(Nucleobase nucleobase)
    {
        return new Nucleotide(nucleobase);
    }

    internal void SetBond(Nucleotide nucleotide, Bond? bond)
    {
        nucleotide.Bond = bond;
    }

    internal void SetOwner(Nucleotide nucleotide, Strand strand)
    {
        nucleotide.Owner = strand;
    }
}