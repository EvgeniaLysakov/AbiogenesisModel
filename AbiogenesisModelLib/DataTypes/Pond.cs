namespace AbiogenesisModel.Lib.DataTypes;

public class Pond
{
    private readonly List<Nucleotide> _nucleotides = [];
    private readonly List<SingleStrand> _singleStrands = [];
    private readonly List<MultiStrand> _multiStrands = [];

    private Nucleotide[]? _freeNucleotides;
    private Nucleotide[]? _allNucleotides;
    private SingleStrand[]? _freeStrands;
    private SingleStrand[]? _allStrands;
    private MultiStrand[]? _allMultiStrands;

    public Nucleotide[] FreeNucleotides => _freeNucleotides ??= [.. _nucleotides.Where(nuc => nuc.Owner == null)];

    public Nucleotide[] AllNucleotides => _allNucleotides ??= [.. _nucleotides];

    public int NucleotideCount => _nucleotides.Count;

    public SingleStrand[] FreeStrands => _freeStrands ??= [.. _singleStrands.Where(strand => strand.Owner == null)];

    public SingleStrand[] AllStrands => _allStrands ??= [.. _singleStrands];

    public int SingleStrandCount => _singleStrands.Count;

    public MultiStrand[] AllMultiStrands => _allMultiStrands ??= [.. _multiStrands];

    public int MultiStrandCount => _multiStrands.Count;

    public void Add(Nucleotide nucleotide)
    {
        _nucleotides.Add(nucleotide);
        _allNucleotides = null;
        _freeNucleotides = null;
    }

    public void Add(SingleStrand strand)
    {
        _singleStrands.Add(strand);
        _allStrands = null;
        _freeStrands = null;
        _freeNucleotides = null;
    }

    public void AddRange(MultiStrand multiStrand)
    {
        _multiStrands.Add(multiStrand);
        _allMultiStrands = null;
        _freeStrands = null;
    }

    public void AddRange(Nucleotide[]? nucleotides)
    {
        if (nucleotides == null)
        {
            return;
        }

        _nucleotides.AddRange(nucleotides);
        _allNucleotides = null;
        _freeNucleotides = null;
    }

    public void AddRange(SingleStrand[]? strands)
    {
        if (strands == null)
        {
            return;
        }

        _singleStrands.AddRange(strands);
        _allStrands = null;
        _freeStrands = null;
        _freeNucleotides = null;
    }

    public void AddRange(MultiStrand[]? multiStrands)
    {
        if (multiStrands == null)
        {
            return;
        }

        _multiStrands.AddRange(multiStrands);
        _allMultiStrands = null;
        _freeStrands = null;
    }

    public void Clear()
    {
        _nucleotides.Clear();
        _singleStrands.Clear();
        _multiStrands.Clear();
        Invalidate();
    }

    public void Invalidate()
    {
        _allNucleotides = null;
        _freeNucleotides = null;
        _allStrands = null;
        _freeStrands = null;
        _allMultiStrands = null;
    }
}