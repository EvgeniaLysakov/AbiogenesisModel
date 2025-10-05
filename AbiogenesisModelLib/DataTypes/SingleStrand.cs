using System.Collections;

namespace AbiogenesisModel.Lib.DataTypes;

public class SingleStrand : IReadOnlyList<Nucleotide>
{
    private readonly List<Nucleotide> _nucleotides = [];

    public SingleStrand(Nucleotide nucleotide)
    {
        Add(nucleotide);
    }

    public SingleStrand(IReadOnlyList<Nucleotide> nucleotides)
    {
        if (!nucleotides.Any())
        {
            throw new ArgumentException("The nucleotides list can't be null or empty", nameof(nucleotides));
        }

        AddRange(nucleotides);
    }

    public Nucleotide[] Nucleotides => [.. _nucleotides];

    public MultiStrand? Owner { get; internal set; }

    public int Count => _nucleotides.Count;

    public Nucleotide this[int index] => _nucleotides[index];

    public IEnumerator<Nucleotide> GetEnumerator()
    {
        return _nucleotides.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(Nucleotide item)
    {
        item.Owner = this;
        _nucleotides.Add(item);
    }

    public void AddRange(IReadOnlyList<Nucleotide> items)
    {
        foreach (var item in items)
        {
            Add(item);
        }
    }

    public void Add(SingleStrand strand)
    {
        AddRange(strand.Nucleotides);
        strand._nucleotides.Clear();
    }

    public SingleStrand Split(int index)
    {
        var nucs = _nucleotides.Skip(index).ToArray();
        _nucleotides.RemoveRange(index, Count - index);
        var secondStrand = new SingleStrand(nucs);
        return secondStrand;
    }
}