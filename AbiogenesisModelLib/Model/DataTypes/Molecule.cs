using AbiogenesisModel.Lib.Attributes;

namespace AbiogenesisModel.Lib.Model;

[DataType]
public class Molecule
{
    internal Molecule(IReadOnlyList<Strand> strands, IReadOnlyList<Bond> bonds)
    {
        Strands = new UnorderedList<Strand>(strands);
        Bonds = new UnorderedList<Bond>(bonds);
    }

    [Owned]
    public IReadOnlyList<Strand> Strands { get; }

    [Owned]
    public IReadOnlyList<Bond> Bonds { get; }
}