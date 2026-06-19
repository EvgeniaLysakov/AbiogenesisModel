using AbiogenesisModel.Lib.Model;
using AbiogenesisModel.Lib.Model.DataTypes;

namespace AbiogenesisModel.Telemetry;

internal static class SimulationWorldTraversalExtension
{
    public static IEnumerable<Stratum> EnumerateStrata(this SimulationWorld simulationWorld)
    {
        foreach (var pond in simulationWorld.Ponds)
        {
            foreach (var stratum in pond.Strata)
            {
                yield return stratum;
            }
        }
    }

    public static IEnumerable<Molecule> EnumerateMolecules(this SimulationWorld simulationWorld)
    {
        foreach (var stratum in simulationWorld.EnumerateStrata())
        {
            foreach (var molecule in stratum.CurrentPopulation.Molecules)
            {
                yield return molecule;
            }

            foreach (var molecule in stratum.SinkingPopulation.Molecules)
            {
                yield return molecule;
            }
        }
    }

    public static IEnumerable<Strand> EnumerateStrands(this SimulationWorld simulationWorld)
    {
        foreach (var molecule in simulationWorld.EnumerateMolecules())
        {
            foreach (var strand in molecule.Strands)
            {
                yield return strand;
            }
        }
    }

    public static IEnumerable<Nucleotide> EnumerateNucleotides(this SimulationWorld simulationWorld)
    {
        foreach (var strand in simulationWorld.EnumerateStrands())
        {
            foreach (var nucleotide in strand.Nucleotides)
            {
                yield return nucleotide;
            }
        }
    }
}