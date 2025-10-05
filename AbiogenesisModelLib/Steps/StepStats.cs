namespace AbiogenesisModel.Lib.Steps;

public abstract record StepStat
{
    public abstract (string Label, double Value)[] ToBarPlotData();
}

public record SingleStrandCreationStat(
    int ConsumedNucleotides,
    int LigationEvents,
    int AddedStrands
) : StepStat
{
    public override (string Label, double Value)[] ToBarPlotData()
    {
        return
        [
            ("Consumed Nucleotides", ConsumedNucleotides),
            ("Ligation Events", LigationEvents),
            ("Added Strands", AddedStrands)
        ];
    }
}

public record NucleotideCreationStat(
    int AddedNucleotides
) : StepStat
{
    public override (string Label, double Value)[] ToBarPlotData()
    {
        return [("Added Nucleotides", AddedNucleotides)];
    }
}