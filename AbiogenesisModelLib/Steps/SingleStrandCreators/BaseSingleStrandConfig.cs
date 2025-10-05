using System.ComponentModel.DataAnnotations;

namespace AbiogenesisModel.Lib.Steps.SingleStrandCreators;

public abstract class BaseSingleStrandConfig : IValidatableObject, ICloneable
{
    [Range(1, int.MaxValue)]
    public int? NucleotidesToConsume { get; set; }

    [Range(1, int.MaxValue)]
    public int? MaxLigationEvents { get; set; }

    [Range(1, int.MaxValue)]
    public int? MaxAddedStrands { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (NucleotidesToConsume == null
            && MaxLigationEvents == null
            && MaxAddedStrands == null)
        {
            yield return new ValidationResult("At least one limitation parameter is required");
        }
    }

    public abstract object Clone();
}