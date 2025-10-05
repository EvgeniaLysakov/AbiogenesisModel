using AbiogenesisModel.Lib.DataTypes;
using AbiogenesisModel.Lib.Pipeline;
using AbiogenesisModel.Lib.Steps;

namespace AbiogenesisModel.Lib;

[Service]
public class AbiogenesisSite(AbiogenesisCycle abiogenesisCycle)
{
    public AbiogenesisCycle AbiogenesisCycle { get; } = abiogenesisCycle;

    public Pond Pond { get; } = new();

    public StepStat[] Loop()
    {
        return AbiogenesisCycle.Loop(Pond);
    }
}