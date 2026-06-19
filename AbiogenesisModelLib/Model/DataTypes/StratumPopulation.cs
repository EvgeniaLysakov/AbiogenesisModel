using AbiogenesisModel.Lib.Attributes;

namespace AbiogenesisModel.Lib.Model;

[DataType]
public class StratumPopulation
{
    internal StratumPopulation()
    {
        MutableMolecules = [];
    }

    [Owned]
    public IReadOnlyList<Molecule> Molecules => MutableMolecules;

    [Runtime]
    internal UnorderedList<Molecule> MutableMolecules { get; }
}
