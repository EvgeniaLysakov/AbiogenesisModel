using AbiogenesisModel.Lib;
using AbiogenesisModel.Lib.DataTypes;
using AbiogenesisModel.Lib.Steps;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace AbiogenesisModel.Test
{
    public class NucleotideCreatorTests
    {
        [Fact]
        public void SingleNucleotideCreatorCreation()
        {
            var site = new AbiogenesisSite();
            site.AbiogenesisCycle.NucleotideCreationLimit.MaxNucleotidesInPond.Should().BeGreaterThan(0);
            site.AbiogenesisCycle.NucleotideCreator.Should().BeOfType<MultinomialNucleotideCreator>();

            var standard = (MultinomialNucleotideCreator)site.AbiogenesisCycle.NucleotideCreator;
            standard.AProbability.Should().BeGreaterThan(0);
            standard.UProbability.Should().BeGreaterThan(0);
            standard.CProbability.Should().BeGreaterThan(0);
            standard.GProbability.Should().BeGreaterThan(0);
        }

        [Fact]
        public void NucleotideCount()
        {
            var site = new AbiogenesisSite();
            site.Pond.AllNucleotides.Length.Should().Be(0);

            var stepStats = site.Loop();
            site.Pond.AllNucleotides.Length.Should().Be(1000);
            var nucleotideCreationStat = stepStats.OfType<NucleotideCreationStat>().FirstOrDefault();
            nucleotideCreationStat.Should().NotBeNull();
            nucleotideCreationStat.AddedNucleotides.Should().Be(1000);

            stepStats = site.Loop();
            site.Pond.AllNucleotides.Length.Should().Be(1000);
            nucleotideCreationStat = stepStats.OfType<NucleotideCreationStat>().FirstOrDefault();
            nucleotideCreationStat.Should().NotBeNull();
            nucleotideCreationStat.AddedNucleotides.Should().Be(0);
        }

        [Theory]
        [InlineData("TestData\\Config\\generalInlined1.yml", 2000, 0.25, 0.25, 0.25, 0.25)]
        [InlineData("TestData\\Config\\generalInlined2.yml", 5000, 0.25, 0.75, 0, 0)]
        public void TestDataCount(string configPath, int count, double aProb, double uProb, double cProb, double gProb)
        {
            var builder = new ConfigurationBuilder().AddYamlFile(configPath, optional: false);
            var abioCycle = new AbiogenesisCycle(builder.Build());
            abioCycle.NucleotideCreationLimit.MaxNucleotidesInPond.Should().Be(count);
            abioCycle.NucleotideCreator.Should().BeOfType<MultinomialNucleotideCreator>();
            var standard = (MultinomialNucleotideCreator)abioCycle.NucleotideCreator;
            standard.AProbability.Should().BeApproximately(aProb, 1e-6);
            standard.UProbability.Should().BeApproximately(uProb, 1e-6);
            standard.CProbability.Should().BeApproximately(cProb, 1e-6);
            standard.GProbability.Should().BeApproximately(gProb, 1e-6);

            var pond = new Pond();
            abioCycle.Loop(pond);
            pond.AllNucleotides.Length.Should().Be(count);
            var delta = (uint)(count * 0.05);
            pond.AllNucleotides.Count(nuc => nuc.Base == Nucleotide.Nucleobase.A).Should().BeCloseTo((int)(count * aProb), delta);
            pond.AllNucleotides.Count(nuc => nuc.Base == Nucleotide.Nucleobase.U).Should().BeCloseTo((int)(count * uProb), delta);
            pond.AllNucleotides.Count(nuc => nuc.Base == Nucleotide.Nucleobase.C).Should().BeCloseTo((int)(count * cProb), delta);
            pond.AllNucleotides.Count(nuc => nuc.Base == Nucleotide.Nucleobase.G).Should().BeCloseTo((int)(count * gProb), delta);
        }
    }
}
