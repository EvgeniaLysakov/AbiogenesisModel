using AbiogenesisModel.Lib.Attributes;
using AbiogenesisModel.Lib.ConfigTypes.ControllerConfigTypes;
using AbiogenesisModel.Lib.Extensions;
using AbiogenesisModel.Lib.Guard;
using AbiogenesisModel.Lib.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace AbiogenesisModel.Lib.Model.Controllers;

[Service]
public class MoleculeController(IConfigFactory<MoleculeControllerConfig> configFactory, StrandController strandController)
    : ConfigurableObject<MoleculeControllerConfig>(configFactory)
{
    public Molecule Create(IReadOnlyList<Strand> strands, IReadOnlyList<Bond> bonds)
    {
        Ensure.That(strands).IsNotEmpty().AllBondsBelongTo(bonds);
        Ensure.That(bonds).AllNucleotidesBelongTo(strands);

        var molecule = new Molecule(strands, bonds);
        UpdateStrandsOwner(molecule);
        return molecule;
    }

    public Molecule Create(Strand strand)
    {
        return Create([strand], Array.Empty<Bond>());
    }

    public Molecule Create(IReadOnlyList<Nucleotide> nucleotides)
    {
        return Create(strandController.Create(nucleotides));
    }

    public Molecule Create(Nucleotide nucleotide)
    {
        return Create(strandController.Create(nucleotide));
    }

    public Molecule Create(IReadOnlyList<Nucleobase> nucleobases)
    {
        return Create(strandController.Create(nucleobases));
    }

    public Molecule Create(Nucleobase nucleobase)
    {
        return Create(strandController.Create(nucleobase));
    }

    private void UpdateStrandsOwner(Molecule molecule)
    {
        foreach (var strand in molecule.Strands)
        {
            strandController.SetOwner(strand, molecule);
        }
    }
}