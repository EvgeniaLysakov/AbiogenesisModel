using AbiogenesisModel.Lib.DataTypes;
using AbiogenesisModel.Lib.Extensions;
using AbiogenesisModel.Lib.Pipeline;
using System.ComponentModel.DataAnnotations;

namespace AbiogenesisModel.Lib.Steps.SingleStrandCreators;

[NamedService("multinomial")]
public class MultinomialSingleStrandCreator(IConfigFactory<MultinomialSingleStrandCreatorConfig> configFactory)
    : ConfigurableObject<MultinomialSingleStrandCreatorConfig>(configFactory), ISingleStrandCreator
{
    private readonly Random _random = new();

    public SingleStrandCreationStat Execute(Pond pond)
    {
        var freeNucleotides = pond.FreeNucleotides.ToList();

        var consumedNucleotides = 0;
        var ligationEvents = 0;
        var addedStrands = 0;

        while (!IsLimitReached(consumedNucleotides, ligationEvents, addedStrands))
        {
            var len = GetRandomLen();

            var nucs = new List<Nucleotide>();

            for (var j = 0; j < len && freeNucleotides.Any(); j++)
            {
                var nuc = _random.Choice(freeNucleotides);
                freeNucleotides.Remove(nuc);
                nucs.Add(nuc);
            }

            if (nucs.Count <= 1)
            {
                break;
            }

            pond.Add(new SingleStrand(nucs));
            addedStrands++;
            consumedNucleotides += nucs.Count;
            ligationEvents += nucs.Count - 1;
        }

        return new SingleStrandCreationStat(consumedNucleotides, ligationEvents, addedStrands);
    }

    private bool IsLimitReached(int consumedNucleotides, int ligationEvents, int addedStrands)
    {
        return !(Configuration.NucleotidesToConsume.IsNullOrBigger(consumedNucleotides)
            && Configuration.MaxLigationEvents.IsNullOrBigger(ligationEvents)
            && Configuration.MaxAddedStrands.IsNullOrBigger(addedStrands));
    }

    private int GetRandomLen()
    {
        var rndVal = _random.NextDouble();

        foreach (var (key, value) in Configuration.Probabilities)
        {
            rndVal -= value;
            if (rndVal <= 0)
            {
                return key;
            }
        }

        return 0;
    }
}

[Config("SingleStrandCreator\\Multinomial")]
public class MultinomialSingleStrandCreatorConfig : BaseSingleStrandConfig
{
    [Required]
    [MinLength(1)]
    public required Dictionary<int, double> Probabilities { get; init; }

    public new IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var res in base.Validate(validationContext))
        {
            yield return res;
        }

        if (Probabilities.Keys.Any(key => key <= 1))
        {
            yield return new ValidationResult("SingleStrand creator must create strands with at least 2 nucleotides");
        }

        if (Probabilities.Values.Any(val => val < 0))
        {
            yield return new ValidationResult("Probability can't be less than 0");
        }

        var total = Probabilities.Values.Sum();
        if (total == 0)
        {
            yield return new ValidationResult("At least one probability should be greater than 0");
        }

        var keys = Probabilities.Keys.ToArray();

        // normalize probabilities
        foreach (var key in keys)
        {
            Probabilities[key] /= total;
        }
    }

    public override object Clone()
    {
        return new MultinomialSingleStrandCreatorConfig()
        {
            NucleotidesToConsume = NucleotidesToConsume,
            MaxLigationEvents = MaxLigationEvents,
            MaxAddedStrands = MaxAddedStrands,
            Probabilities = new Dictionary<int, double>(Probabilities)
        };
    }
}