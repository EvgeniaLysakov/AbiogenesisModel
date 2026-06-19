using AbiogenesisModel.Lib.Attributes;
using AbiogenesisModel.Lib.ConfigTypes.ControllerConfigTypes;
using AbiogenesisModel.Lib.Guard;
using AbiogenesisModel.Lib.Pipeline;

namespace AbiogenesisModel.Lib.Model.Controllers;

[Service]
public class StrandController(IConfigFactory<StrandControllerConfig> configFactory, NucleotideController nucleotideController)
    : ConfigurableObject<StrandControllerConfig>(configFactory)
{
    public Strand Create(IReadOnlyList<Nucleotide> nucleotides)
    {
        Ensure.That(nucleotides).IsNotEmpty();

        var strand = new Strand(nucleotides);
        UpdateNucleotidesOwner(strand);
        return strand;
    }

    public Strand Create(Nucleotide nucleotide)
    {
        return Create([nucleotide]);
    }

    public Strand Create(IReadOnlyList<Nucleobase> nucleobases)
    {
        return Create(nucleobases.Select(nucleotideController.Create).ToArray());
    }

    public Strand Create(Nucleobase nucleobase)
    {
        return Create(nucleotideController.Create(nucleobase));
    }

    public Strand Merge(Strand strand1, Strand strand2)
    {
        Ensure.That(strand1).IsNotNull().IsNotSameAs(strand2);
        Ensure.That(strand2).IsNotNull();

        var strand = new Strand(strand1.Nucleotides.Concat(strand2.Nucleotides).ToArray());
        UpdateNucleotidesOwner(strand);
        return strand;
    }

    public Strand[] Split(Strand strand, int splitLocation)
    {
        Ensure.That(strand).IsNotNull();
        Ensure.That(splitLocation).IsInRangeOfIndexes(strand.Nucleotides).DoesNotSatisfy(i => i == 0 || i == strand.Nucleotides.Length - 1, $"{nameof(splitLocation)} must be not equal to the first or last index of {nameof(strand)}.{nameof(strand.Nucleotides)}");

        var strand1 = new Strand(strand.Nucleotides.Take(splitLocation).ToArray());
        UpdateNucleotidesOwner(strand1);

        var strand2 = new Strand(strand.Nucleotides.Skip(splitLocation).ToArray());
        UpdateNucleotidesOwner(strand2);

        return [strand1, strand2];
    }

    internal void SetOwner(Strand strand, Molecule molecule)
    {
        strand.Owner = molecule;
    }

    private void UpdateNucleotidesOwner(Strand strand)
    {
        foreach (var nucleotide in strand.Nucleotides)
        {
            nucleotideController.SetOwner(nucleotide, strand);
        }
    }
}