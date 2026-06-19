using AbiogenesisModel.Lib.Attributes;

namespace AbiogenesisModel.Lib.Model;

[DataType]
public class Pond
{
    internal Pond(IReadOnlyList<Stratum> strata)
    {
        Strata = strata;
    }

    [Owned]
    public IReadOnlyList<Stratum> Strata { get; }
}