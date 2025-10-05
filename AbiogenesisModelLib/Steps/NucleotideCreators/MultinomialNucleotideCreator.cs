using System.ComponentModel.DataAnnotations;
using AbiogenesisModel.Lib.DataTypes;
using AbiogenesisModel.Lib.Extensions;
using AbiogenesisModel.Lib.Pipeline;

namespace AbiogenesisModel.Lib.Steps.NucleotideCreators;

[NamedService("multinomial")]
public class MultinomialNucleotideCreator(IConfigFactory<MultinomialNucleotideCreatorConfig> configFactory)
    : ConfigurableObject<MultinomialNucleotideCreatorConfig>(configFactory), INucleotideCreator
{
    private readonly Random _random = new();

    private Nucleotide.Nucleobase GetRandomNucleobase()
    {
        var rndVal = _random.NextDouble();

        foreach (var (key, value) in Configuration.Probabilities)
        {
            rndVal -= value;
            if (rndVal <= 0)
            {
                switch (key)
                {
                    case "A":
                        return Nucleotide.Nucleobase.A;
                    case "U":
                        return Nucleotide.Nucleobase.U;
                    case "C":
                        return Nucleotide.Nucleobase.C;
                    case "G":
                        return Nucleotide.Nucleobase.G;
                }
            }
        }

        throw new InvalidOperationException("Failed to get a random nucleobase");
    }

    public NucleotideCreationStat Execute(Pond pond)
    {
        var addedNucleotides = 0;
        while (!IsLimitReached(addedNucleotides, pond.NucleotideCount))
        {
            var nucleobase = GetRandomNucleobase();

            pond.Add(new Nucleotide(nucleobase));
            addedNucleotides++;
        }

        return new NucleotideCreationStat(addedNucleotides);
    }

    private bool IsLimitReached(int addedNucleotides, int currentNucleotideCount)
    {
        return !(Configuration.MaxAddedNucleotides.IsNullOrBigger(addedNucleotides)
            && Configuration.MaxNucleotidesInPond.IsNullOrBigger(currentNucleotideCount));
    }
}

[Config("NucleotideCreator")]
public class MultinomialNucleotideCreatorConfig : IValidatableObject, ICloneable
{
    [Required]
    [MinLength(1)]
    public required Dictionary<string, double> Probabilities { get; init; }

    [Range(1, int.MaxValue)]
    public int? MaxAddedNucleotides { get; set; }

    [Range(1, int.MaxValue)]
    public int? MaxNucleotidesInPond { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MaxAddedNucleotides == null && MaxNucleotidesInPond == null)
        {
            yield return new ValidationResult("At least one limitation parameter is required");
        }

        var totalProbability = Probabilities.Values.Sum();
        if (totalProbability == 0)
        {
            yield return new ValidationResult("The total probability can't be 0");
        }

        if (Probabilities.Keys.Any(key => !Enum.TryParse<Nucleotide.Nucleobase>(key, out var _)))
        {
            yield return new ValidationResult("The collection contains foreign key");
        }

        // normalize probabilities
        foreach (var key in Probabilities.Keys)
        {
            Probabilities[key] /= totalProbability;
        }
    }

    public object Clone()
    {
        return new MultinomialNucleotideCreatorConfig()
        {
            Probabilities = new Dictionary<string, double>(Probabilities),
            MaxAddedNucleotides = MaxAddedNucleotides,
            MaxNucleotidesInPond = MaxNucleotidesInPond
        };
    }
}