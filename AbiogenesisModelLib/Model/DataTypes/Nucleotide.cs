using AbiogenesisModel.Lib.Attributes;

namespace AbiogenesisModel.Lib.Model;

[DataType]
public class Nucleotide
{
    internal Nucleotide(Nucleobase @base)
    {
        Base = @base;
    }

    [Owned]
    public Nucleobase Base { get; }

    [Knows]
    public Bond? Bond { get; internal set; }

    [Knows]
    public Strand? Owner { get; internal set; }

    public override string ToString()
    {
        return Base.ToString();
    }
}