using AbiogenesisModel.Lib.DataTypes;
using AbiogenesisModel.Lib.Pipeline;
using AbiogenesisModel.Lib.Steps;

namespace AbiogenesisModel.Lib;

[Service]
public class AbiogenesisCycle : ConfigurableObject<AbiogenesisCycleConfig>
{
    public AbiogenesisCycle(IKeyedFactory<INucleotideCreator> nucleotideCreatorFactory,
                            IKeyedFactory<ISingleStrandCreator> singleStrandCreatorFactory,
                            IConfigFactory<AbiogenesisCycleConfig> configFactory)
        : base(configFactory)
    {
        NucleotideCreator = nucleotideCreatorFactory.Get(Configuration.NucleotideCreator);
        SingleStrandCreator = singleStrandCreatorFactory.Get(Configuration.SingleStrandCreator);
    }

    public INucleotideCreator NucleotideCreator { get; }

    public ISingleStrandCreator SingleStrandCreator { get; }

    public StepStat[] Loop(Pond pond)
    {
        var stats = new List<StepStat>();
        StepStat stat = NucleotideCreator.Execute(pond);
        stats.Add(stat);

        stat = SingleStrandCreator.Execute(pond);
        stats.Add(stat);

        return stats.ToArray();
    }
}

[Config("Cycle")]
public class AbiogenesisCycleConfig : ICloneable
{
    public string NucleotideCreator { get; init; } = "default";
    public string SingleStrandCreator { get; init; } = "default";

    public object Clone()
    {
        return new AbiogenesisCycleConfig() { NucleotideCreator = NucleotideCreator, SingleStrandCreator = SingleStrandCreator };
    }
}