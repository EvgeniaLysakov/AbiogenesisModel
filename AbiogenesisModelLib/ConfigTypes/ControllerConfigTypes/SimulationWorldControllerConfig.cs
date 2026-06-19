using AbiogenesisModel.Lib.Attributes;
using System.ComponentModel.DataAnnotations;

namespace AbiogenesisModel.Lib.ConfigTypes.ControllerConfigTypes;

[Config("SimulationWorldController")]
public class SimulationWorldControllerConfig : ICloneable
{
    [Required]
    [Range(1, int.MaxValue)]
    public required int PondsNum { get; init; }

    public object Clone()
    {
        return new SimulationWorldControllerConfig() { PondsNum = PondsNum };
    }
}