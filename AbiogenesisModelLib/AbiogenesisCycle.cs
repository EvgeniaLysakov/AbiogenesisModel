using AbiogenesisModel.Lib.Configurability;
using AbiogenesisModel.Lib.DataTypes;
using AbiogenesisModel.Lib.Steps;
using Microsoft.Extensions.Configuration;

namespace AbiogenesisModel.Lib
{
    public class AbiogenesisCycle : ConfigurableObject
    {
        public AbiogenesisCycle()
        {
        }

        public AbiogenesisCycle(IConfiguration config)
            : base(config)
        {
        }

        [Configurable("nucleotideCreator")]
        public INucleotideCreator NucleotideCreator { get; internal set; } = null!;

        [Configurable("nucleotideCreationLimit")]
        public NucleotideCreationLimit NucleotideCreationLimit { get; internal set; } = null!;

        [Configurable("singleStrandCreator")]
        public ISingleStrandCreator SingleStrandCreator { get; internal set; } = null!;

        [Configurable("singleStrandCreationLimit")]
        public SingleStrandCreationLimit SingleStrandCreationLimit { get; internal set; } = null!;

        public StepStat[] Loop(Pond pond)
        {
            var stats = new List<StepStat>();
            StepStat stat = NucleotideCreator.Create(NucleotideCreationLimit, pond);
            stats.Add(stat);

            stat = SingleStrandCreator.Create(SingleStrandCreationLimit, pond);
            stats.Add(stat);

            return stats.ToArray();
        }
    }
}
