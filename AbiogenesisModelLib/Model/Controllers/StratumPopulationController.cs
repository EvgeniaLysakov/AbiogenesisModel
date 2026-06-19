using AbiogenesisModel.Lib.Attributes;
using AbiogenesisModel.Lib.ConfigTypes.ControllerConfigTypes;
using AbiogenesisModel.Lib.Guard;
using AbiogenesisModel.Lib.Pipeline;

namespace AbiogenesisModel.Lib.Model.Controllers;

[Service]
public class StratumPopulationController(IConfigFactory<StratumPopulationControllerConfig> configFactory, MoleculeController moleculeController)
    : ConfigurableObject<StratumPopulationControllerConfig>(configFactory)
{
    public StratumPopulation Create()
    {
        var stratumPopulation = new StratumPopulation();
        return stratumPopulation;
    }

    public void Add(StratumPopulation stratumPopulation, Molecule molecule)
    {
        Ensure.That(molecule).IsNotNull().IsNotIncludedIn(stratumPopulation);

        stratumPopulation.MutableMolecules.Add(molecule);
    }

    public void Add(StratumPopulation stratumPopulation, IReadOnlyList<Nucleobase> nucleobases)
    {
        stratumPopulation.MutableMolecules.Add(moleculeController.Create(nucleobases));
    }

    internal void Remove(StratumPopulation stratumPopulation, Molecule molecule)
    {
        Ensure.That(molecule).IsNotNull().IsIncludedIn(stratumPopulation);

        stratumPopulation.MutableMolecules.Remove(molecule);
    }

    internal void Clear(StratumPopulation stratumPopulation)
    {
        stratumPopulation.MutableMolecules.Clear();
    }
}