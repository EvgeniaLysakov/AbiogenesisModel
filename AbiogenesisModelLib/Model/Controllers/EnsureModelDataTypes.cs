using AbiogenesisModel.Lib.Guard;
using System.Runtime.CompilerServices;

namespace AbiogenesisModel.Lib.Model.Controllers;

public static class EnsureModelDataTypes
{
    public static Ensure<Nucleotide> IsBonded(this Ensure<Nucleotide> ensure, string? message = null)
    {
        if (ensure.Value.Bond == null)
        {
            throw new InvalidOperationException(message ?? $"{ensure.Name} must be bonded.");
        }

        return ensure;
    }

    public static Ensure<Nucleotide> IsNotBonded(this Ensure<Nucleotide> ensure, string? message = null)
    {
        if (ensure.Value.Bond != null)
        {
            throw new InvalidOperationException(message ?? $"{ensure.Name} must be not bonded.");
        }

        return ensure;
    }

    public static Ensure<IReadOnlyList<Bond>> AllNucleotidesBelongTo(this Ensure<IReadOnlyList<Bond>> ensure, IReadOnlyList<Strand> strands, [CallerArgumentExpression("strands")] string strandsName = "???", string? message = null)
    {
        var strandSet = strands as ISet<Strand> ?? new HashSet<Strand>(strands);
        var hasForeignOwner = ensure.Value.Any(bond =>
        {
            return bond.Nucleotides.Any(nucleotide =>
            {
                var owner = nucleotide.Owner;
                return owner is not null && !strandSet.Contains(owner);
            });
        });

        if (hasForeignOwner)
        {
            throw new InvalidOperationException(message ?? $"All nucleotides in {ensure.Name} must belong to {strandsName}.");
        }

        return ensure;
    }

    public static Ensure<IReadOnlyList<Strand>> AllBondsBelongTo(this Ensure<IReadOnlyList<Strand>> ensure, IReadOnlyList<Bond> bonds, [CallerArgumentExpression("bonds")] string bondsName = "???", string? message = null)
    {
        var bondSet = bonds as ISet<Bond> ?? new HashSet<Bond>(bonds);
        var hasForeignBond = ensure.Value.Any(strand =>
        {
            return strand.Nucleotides.Any(nucleotide =>
            {
                var bond = nucleotide.Bond;
                return bond is not null && !bondSet.Contains(bond);
            });
        });

        if (hasForeignBond)
        {
            throw new InvalidOperationException(message ?? $"All bonds in {ensure.Name} must belong to {bondsName}.");
        }

        return ensure;
    }

    public static Ensure<Molecule> IsIncludedIn(this Ensure<Molecule> ensure, StratumPopulation stratumPopulation, [CallerArgumentExpression("stratumPopulation")] string stratumPopulationName = "???", string? message = null)
    {
        var molecule = ensure.Value;
        var isIncluded = stratumPopulation.Molecules.Contains(molecule);
        if (!isIncluded)
        {
            throw new InvalidOperationException(message ?? $"{ensure.Name} must be included in {stratumPopulationName}.");
        }

        return ensure;
    }

    public static Ensure<Molecule> IsNotIncludedIn(this Ensure<Molecule> ensure, StratumPopulation stratumPopulation, [CallerArgumentExpression("stratumPopulation")] string stratumPopulationName = "???", string? message = null)
    {
        var molecule = ensure.Value;
        var isIncluded = stratumPopulation.Molecules.Contains(molecule);
        if (isIncluded)
        {
            throw new InvalidOperationException(message ?? $"{ensure.Name} must be not included in {stratumPopulationName}.");
        }

        return ensure;
    }
}