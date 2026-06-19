using AbiogenesisModel.Lib.Attributes;

namespace AbiogenesisModel.Lib.Model;

[DataType]
public class Bond
{
    internal Bond(Nucleotide nucleotide0, Nucleotide nucleotide1)
    {
        Nucleotides = [nucleotide0, nucleotide1];
    }

    [Knows]
    public IReadOnlyList<Nucleotide> Nucleotides { get; }
}
