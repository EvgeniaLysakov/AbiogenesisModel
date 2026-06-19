using AbiogenesisModel.Lib.ConfigTypes.ControllerConfigTypes;
using AbiogenesisModel.Lib.Model;
using AbiogenesisModel.Lib.Model.Controllers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace AbiogenesisModel.Test;

public class ModelDataControllersTests : BaseTests
{
    [Fact]
    public void MoleculeCreationTest()
    {
        var provider = InitServiceCollectionFromFiles();

        var moleculeController = provider.GetRequiredService<MoleculeController>();
        var nucleotideController = provider.GetRequiredService<NucleotideController>();
        var strandController = provider.GetRequiredService<StrandController>();

        var m1 = moleculeController.Create(Nucleobase.A);
        CheckMolecule(m1, [Nucleobase.A]);

        var m2 = moleculeController.Create([Nucleobase.U, Nucleobase.G, Nucleobase.A]);
        CheckMolecule(m2, [Nucleobase.U, Nucleobase.G, Nucleobase.A]);

        var m3 = moleculeController.Create(nucleotideController.Create(Nucleobase.G));
        CheckMolecule(m3, [Nucleobase.G]);

        var m4 = moleculeController.Create([nucleotideController.Create(Nucleobase.A), nucleotideController.Create(Nucleobase.C)]);
        CheckMolecule(m4, [Nucleobase.A, Nucleobase.C]);

        var m5 = moleculeController.Create(strandController.Create([Nucleobase.A, Nucleobase.C, Nucleobase.G]));
        CheckMolecule(m5, [Nucleobase.A, Nucleobase.C, Nucleobase.G]);
    }

    [Fact]
    public void FileConfiguredSimulationWorldCreationTest()
    {
        var provider = InitServiceCollectionFromFiles();

        CreateSimulationWorld(provider, 5, 20, 10);
    }

    [Theory]
    [InlineData(3, 15.5, 5)]
    [InlineData(1, 0, 1)]
    [InlineData(10, 100, 20)]
    public void YamlConfiguredSimulationWorldCreationTest(int pondsNum, double initialTemperature, int strataNum)
    {
        var configuredTypes = new Dictionary<Type, string>
        {
            [typeof(SimulationWorldControllerConfig)] = $"PondsNum: {pondsNum}",
            [typeof(ExternalEnvironmentControllerConfig)] = $"InitialTemperature: {initialTemperature}",
            [typeof(PondControllerConfig)] = $"StrataNum: {strataNum}"
        };

        var emptyTypes = GetConfigTypes().Except(configuredTypes.Keys.ToArray()).ToArray();

        var provider = InitServiceCollectionFromYamls(configuredTypes, emptyTypes);

        CreateSimulationWorld(provider, pondsNum, initialTemperature, strataNum);
    }

    private static void CreateSimulationWorld(ServiceProvider provider, int pondsNum, double initialTemperature, int strataNum)
    {
        var controller = provider.GetRequiredService<SimulationWorldController>();
        var world = controller.Create();
        world.Should().NotBeNull();
        world.ExternalEnvironment.Should().NotBeNull();
        world.ExternalEnvironment.Temperature.Should().Be(initialTemperature);
        world.Ponds.Should().NotBeNull();
        world.Ponds.Should().HaveCount(pondsNum);
        world.Ponds.Should().AllSatisfy(pond =>
        {
            pond.Should().NotBeNull();
            pond.Strata.Should().NotBeNull();
            pond.Strata.Should().HaveCount(strataNum);
            pond.Strata.Should().AllSatisfy(stratum =>
            {
                stratum.Should().NotBeNull();
                stratum.CurrentPopulation.Should().NotBeNull();
                stratum.CurrentPopulation.Molecules.Should().NotBeNull();
                stratum.CurrentPopulation.Molecules.Should().BeEmpty();
            });
        });
    }

    private static void CheckMolecule(Molecule molecule, IReadOnlyList<Nucleobase> nucleobases)
    {
        molecule.Should().NotBeNull();
        molecule.Bonds.Should().BeEmpty();
        molecule.Strands.Should().HaveCount(1);
        molecule.Strands[0].Owner.Should().Be(molecule);
        molecule.Strands[0].Nucleotides.Should().HaveCount(nucleobases.Count);
        molecule.Strands[0].Nucleotides.Should().AllSatisfy(n =>
        {
            n.Bond.Should().BeNull();
            n.Owner.Should().Be(molecule.Strands[0]);
        });

        for (var i = 0; i < nucleobases.Count; i++)
        {
            molecule.Strands[0].Nucleotides[i].Base.Should().Be(nucleobases[i]);
        }
    }
}