using AbiogenesisModel.Lib.DataTypes;

namespace AbiogenesisModel.Lib.Steps
{
    public interface IStep
    {
        void ValidateAndInit();
    }

    public interface INucleotideCreator : IStep
    {
        NucleotideCreationStat Create(NucleotideCreationLimit limit, Pond pond);
    }

    public interface ISingleStrandCreator : IStep
    {
        SingleStrandCreationStat Create(SingleStrandCreationLimit limit, Pond pond);
    }
}
