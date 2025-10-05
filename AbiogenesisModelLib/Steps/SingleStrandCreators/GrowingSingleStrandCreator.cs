using AbiogenesisModel.Lib.DataTypes;
using AbiogenesisModel.Lib.Extensions;
using AbiogenesisModel.Lib.Pipeline;
using System.ComponentModel.DataAnnotations;

namespace AbiogenesisModel.Lib.Steps.SingleStrandCreators;

[NamedService("growing")]
public class GrowingSingleStrandCreator(IConfigFactory<GrowingSingleStrandCreatorConfig> configFactory)
    : ConfigurableObject<GrowingSingleStrandCreatorConfig>(configFactory), ISingleStrandCreator
{
    private readonly Random _random = new();

    public SingleStrandCreationStat Execute(Pond pond)
    {
        var freeNucleotides = pond.FreeNucleotides.ToList();
        var activeStrands = Configuration.ActivateAllStrandsInPond ? pond.AllStrands.ToDictionary(strand => strand.Last()) : [];
        var activeNucleotides = freeNucleotides.Concat(activeStrands.Keys).ToList();

        pond.Invalidate();

        var consumedNucleotides = 0;
        var ligationEvents = 0;
        var addedStrands = 0;

        while (!IsLimitReached(consumedNucleotides, ligationEvents, addedStrands))
        {
            if (activeNucleotides.Count == 0)
            {
                break;
            }

            var nuc_3 = _random.Choice(activeNucleotides);
            freeNucleotides.Remove(nuc_3);
            activeNucleotides.Remove(nuc_3);

            if (freeNucleotides.Count == 0)
            {
                break;
            }

            var nuc_5 = _random.Choice(freeNucleotides);
            freeNucleotides.Remove(nuc_5);
            activeNucleotides.Remove(nuc_5);

            if (!activeStrands.Remove(nuc_3, out var strand))
            {
                strand = new SingleStrand(nuc_3);
                pond.Add(strand);
                consumedNucleotides++;
                addedStrands++;
            }

            strand.Add(nuc_5);
            consumedNucleotides++;
            ligationEvents++;

            activeStrands.Add(nuc_5, strand);
        }

        return new SingleStrandCreationStat(consumedNucleotides, ligationEvents, addedStrands);
    }

    private bool IsLimitReached(int consumedNucleotides, int ligationEvents, int addedStrands)
    {
        return !(Configuration.NucleotidesToConsume.IsNullOrBigger(consumedNucleotides)
                 && Configuration.MaxLigationEvents.IsNullOrBigger(ligationEvents)
                 && Configuration.MaxAddedStrands.IsNullOrBigger(addedStrands));
    }
}

[Config("SingleStrandCreator\\Growing")]
public class GrowingSingleStrandCreatorConfig : BaseSingleStrandConfig
{
    public bool ActivateAllStrandsInPond { get; set; }

    public new IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var res in base.Validate(validationContext))
        {
            yield return res;
        }

        if (NucleotidesToConsume == null && MaxLigationEvents == null && MaxAddedStrands == null)
        {
            yield return new ValidationResult("At least one limitation parameter is required");
        }
    }

    public override object Clone()
    {
        return new GrowingSingleStrandCreatorConfig()
        {
            ActivateAllStrandsInPond = ActivateAllStrandsInPond,
            NucleotidesToConsume = NucleotidesToConsume,
            MaxLigationEvents = MaxLigationEvents,
            MaxAddedStrands = MaxAddedStrands
        };
    }
}