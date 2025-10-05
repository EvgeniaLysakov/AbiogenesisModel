using AbiogenesisModel.Lib.DataTypes;

namespace AbiogenesisModel.Lib.Steps;

public interface IStep<out T>
    where T : StepStat
{
    T Execute(Pond pond);
}

public interface INucleotideCreator : IStep<NucleotideCreationStat>
{
}

public interface ISingleStrandCreator : IStep<SingleStrandCreationStat>
{
}