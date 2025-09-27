using AbiogenesisModel.Lib.DataTypes;
using AbiogenesisModel.Lib.Steps;

namespace AbiogenesisModel.Lib
{
    public class AbiogenesisSite
    {
        public AbiogenesisCycle AbiogenesisCycle { get; } = new AbiogenesisCycle();

        public Pond Pond { get; } = new();

        public StepStat[] Loop()
        {
            return AbiogenesisCycle.Loop(Pond);
        }
    }
}
