using AbiogenesisModel.Lib.Extensions;

namespace AbiogenesisModel.Lib.Steps
{
    public record NucleotideCreationLimit
    {
        public NucleotideCreationLimit(int? MaxAddedNucleotides = null,
                                        int? MaxNucleotidesInPond = null)
        {
            if (MaxAddedNucleotides is null
                && MaxNucleotidesInPond is null)
            {
                throw new ArgumentException(
                    $"At least one limit must be specified in {nameof(NucleotideCreationLimit)}");
            }

            this.MaxAddedNucleotides = MaxAddedNucleotides;
            this.MaxNucleotidesInPond = MaxNucleotidesInPond;
        }

        public int? MaxAddedNucleotides { get; init; }

        public int? MaxNucleotidesInPond { get; init; }

        public bool IsReached(int addedNucleotides, int nucleotidesInPond)
        {
            return !(MaxAddedNucleotides.IsNullOrBigger(addedNucleotides)
                    && MaxNucleotidesInPond.IsNullOrBigger(nucleotidesInPond));
        }
    }

    public record SingleStrandCreationLimit
    {
        public SingleStrandCreationLimit(int? NucleotidesToConsume = null,
                                          int? MaxLigationEvents = null,
                                          int? MaxAddedStrands = null)
        {
            if (NucleotidesToConsume is null
                && MaxLigationEvents is null
                && MaxAddedStrands is null)
            {
                throw new ArgumentException(
                    $"At least one limit must be specified in {nameof(SingleStrandCreationLimit)}");
            }

            this.NucleotidesToConsume = NucleotidesToConsume;
            this.MaxLigationEvents = MaxLigationEvents;
            this.MaxAddedStrands = MaxAddedStrands;
        }

        public int? NucleotidesToConsume { get; init; }

        public int? MaxLigationEvents { get; init; }

        public int? MaxAddedStrands { get; init; }

        public bool IsReached(int consumedNucleotides, int ligationEvents, int addedStrands)
        {
            return !(NucleotidesToConsume.IsNullOrBigger(consumedNucleotides)
                     && MaxLigationEvents.IsNullOrBigger(ligationEvents)
                     && MaxAddedStrands.IsNullOrBigger(addedStrands));
        }
    }
}
