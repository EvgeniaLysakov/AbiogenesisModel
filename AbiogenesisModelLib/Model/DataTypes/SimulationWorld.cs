using AbiogenesisModel.Lib.Attributes;

namespace AbiogenesisModel.Lib.Model.DataTypes;

[DataType]
public class SimulationWorld
{
    internal SimulationWorld(ExternalEnvironment externalEnvironment, IReadOnlyList<Pond> ponds)
    {
        ExternalEnvironment = externalEnvironment;
        Ponds = ponds;
    }

    [Owned]
    public ExternalEnvironment ExternalEnvironment { get; }

    [Owned]
    public IReadOnlyList<Pond> Ponds { get; }
}