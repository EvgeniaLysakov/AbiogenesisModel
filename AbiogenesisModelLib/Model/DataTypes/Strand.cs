using AbiogenesisModel.Lib.Attributes;
using AbiogenesisModel.Lib.Extensions;

namespace AbiogenesisModel.Lib.Model;

[DataType]
public class Strand
{
    private readonly Nucleotide[] _nucleotides;

    internal Strand(IReadOnlyList<Nucleotide> nucleotides)
    {
        _nucleotides = nucleotides.ToArrayOrCast();
    }

    [Owned]
    public Nucleotide[] Nucleotides => [.. _nucleotides];

    [Knows]
    public Molecule? Owner { get; internal set; }

    [Knows]
    public Nucleotide this[int index] => _nucleotides[index];
}