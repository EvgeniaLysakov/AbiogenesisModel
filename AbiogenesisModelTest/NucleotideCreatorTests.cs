using AbiogenesisModel.Lib;
using AbiogenesisModel.Lib.Steps;
using AbiogenesisModel.Lib.Steps.NucleotideCreators;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace AbiogenesisModel.Test
{
    public class NucleotideCreatorTests : BaseTest
    {
        [Fact]
        public void SingleNucleotideCreatorCreation()
        {
            var provider = InitServiceCollection();
            var site = provider.GetRequiredService<AbiogenesisSite>();
            site.AbiogenesisCycle.NucleotideCreator.Should().BeOfType<MultinomialNucleotideCreator>();

            var config = ((MultinomialNucleotideCreator)site.AbiogenesisCycle.NucleotideCreator).Configuration;
            config.MaxAddedNucleotides.Should().BeGreaterThan(0);
            config.Probabilities["A"].Should().BeGreaterThan(0);
            config.Probabilities["U"].Should().BeGreaterThan(0);
            config.Probabilities["G"].Should().BeGreaterThan(0);
            config.Probabilities["C"].Should().BeGreaterThan(0);
        }

        [Fact]
        public void NucleotideCount()
        {
            var provider = InitServiceCollection();
            var site = provider.GetRequiredService<AbiogenesisSite>();
            ((MultinomialNucleotideCreator)site.AbiogenesisCycle.NucleotideCreator).Configuration.MaxNucleotidesInPond = 100;
            site.Pond.AllNucleotides.Length.Should().Be(0);

            var stepStats = site.Loop();
            site.Pond.AllNucleotides.Length.Should().Be(100);
            var nucleotideCreationStat = stepStats.OfType<NucleotideCreationStat>().FirstOrDefault();
            nucleotideCreationStat.Should().NotBeNull();
            nucleotideCreationStat.AddedNucleotides.Should().Be(100);

            stepStats = site.Loop();
            site.Pond.AllNucleotides.Length.Should().Be(100);
            nucleotideCreationStat = stepStats.OfType<NucleotideCreationStat>().FirstOrDefault();
            nucleotideCreationStat.Should().NotBeNull();
            nucleotideCreationStat.AddedNucleotides.Should().Be(0);
        }
    }
}
