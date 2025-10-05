using AbiogenesisModel.Lib.DataTypes;
using AbiogenesisModel.Lib.Pipeline;

namespace AbiogenesisModel.Lib.Steps.SingleStrandCreators;

[NamedService("default")]
public class DummySingleStrandCreator : ISingleStrandCreator
{
    public SingleStrandCreationStat Execute(Pond pond)
    {
        return new SingleStrandCreationStat(0, 0, 0);
    }
}