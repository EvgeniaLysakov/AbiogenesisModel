using AbiogenesisModel.Lib.DataTypes;
using AbiogenesisModel.Lib.Pipeline;

namespace AbiogenesisModel.Lib.Steps.NucleotideCreators;

[NamedService("default")]
public class DummyNucleotideCreator : INucleotideCreator
{
    public NucleotideCreationStat Execute(Pond pond)
    {
        return new NucleotideCreationStat(0);
    }
}