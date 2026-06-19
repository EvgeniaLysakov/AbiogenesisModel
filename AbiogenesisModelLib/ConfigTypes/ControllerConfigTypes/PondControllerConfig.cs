using AbiogenesisModel.Lib.Attributes;
using System.ComponentModel.DataAnnotations;

namespace AbiogenesisModel.Lib.ConfigTypes.ControllerConfigTypes;

[Config("PondController")]
public class PondControllerConfig : ICloneable
{
    [Required]
    [Range(1, int.MaxValue)]
    public required int StrataNum { get; init; }

    public object Clone()
    {
        return new PondControllerConfig() { StrataNum = StrataNum };
    }
}
