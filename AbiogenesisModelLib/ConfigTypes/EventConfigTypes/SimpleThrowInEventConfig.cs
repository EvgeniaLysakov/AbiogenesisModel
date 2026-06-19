using AbiogenesisModel.Lib.Attributes;
using AbiogenesisModel.Lib.Model;
using System.ComponentModel.DataAnnotations;

namespace AbiogenesisModel.Lib.ConfigTypes.EventConfigTypes;

[Config("SimpleThrowInEvent")]
public class SimpleThrowInEventConfig : ICloneable, IValidatableObject
{
    [Required]
    [MinLength(1)]
    public required Dictionary<Nucleobase, double> NucleobaseProbabilities { get; init; }

    [Required]
    [MinLength(1)]
    public required Dictionary<int, double> StrandLengthProbabilities { get; init; }

    [Required]
    [Range(1, double.MaxValue)]
    public required double StrandsPerEventAverage { get; init; }

    [Range(0, double.MaxValue)]
    public double StrandsPerEventVariance { get; set; }

    public object Clone()
    {
        return new SimpleThrowInEventConfig()
        {
            NucleobaseProbabilities = new Dictionary<Nucleobase, double>(NucleobaseProbabilities),
            StrandLengthProbabilities = new Dictionary<int, double>(StrandLengthProbabilities),
            StrandsPerEventAverage = StrandsPerEventAverage,
            StrandsPerEventVariance = StrandsPerEventVariance
        };
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var totalNucleobaseProbability = NucleobaseProbabilities.Values.Sum();
        if (totalNucleobaseProbability == 0)
        {
            yield return new ValidationResult(
                "The total nucleobase probability can't be 0",
                [nameof(NucleobaseProbabilities)]);
        }

        var totalStrandLengthProbability = StrandLengthProbabilities.Values.Sum();
        if (totalStrandLengthProbability == 0)
        {
            yield return new ValidationResult(
                "The total strand length probability can't be 0",
                [nameof(StrandLengthProbabilities)]);
        }

        if (StrandLengthProbabilities.Keys.Any(key => key <= 0))
        {
            yield return new ValidationResult(
                "The strand length probabilities collection contains invalid key",
                [nameof(StrandLengthProbabilities)]);
        }

        if (StrandsPerEventVariance > StrandsPerEventAverage * StrandsPerEventAverage)
        {
            yield return new ValidationResult(
                $"{nameof(StrandsPerEventVariance)} cannot be greater than the square of {nameof(StrandsPerEventAverage)}.",
                [nameof(StrandsPerEventVariance), nameof(StrandsPerEventAverage)]);
        }

        // normalize probabilities
        foreach (var key in NucleobaseProbabilities.Keys)
        {
            NucleobaseProbabilities[key] /= totalNucleobaseProbability;
        }

        foreach (var key in StrandLengthProbabilities.Keys)
        {
            StrandLengthProbabilities[key] /= totalStrandLengthProbability;
        }
    }
}