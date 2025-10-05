namespace AbiogenesisModel.Lib.DataTypes;

public class Nucleotide(Nucleotide.Nucleobase @base)
{
    public enum Nucleobase { A, U, C, G }

    public Nucleobase Base { get; } = @base;

    public Nucleotide? Bonded { get; set; }

    public SingleStrand? Owner { get; internal set; }

    public override string ToString()
    {
        return Base.ToString();
    }
}