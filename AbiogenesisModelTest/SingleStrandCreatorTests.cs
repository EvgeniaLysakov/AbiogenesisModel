using AbiogenesisModel.Lib;
using AbiogenesisModel.Lib.Steps;
using FluentAssertions;

namespace AbiogenesisModel.Test
{
    public class SingleStrandCreatorTests
    {
        [Fact]
        public void NucleotideCount()
        {
            var site = new AbiogenesisSite();
            site.Pond.AllNucleotides.Length.Should().Be(0);

            var stepStats = site.Loop();
            site.Pond.AllNucleotides.Length.Should().Be(1000);
            site.Pond.FreeNucleotides.Length.Should().BeLessThan(1000);
            site.Pond.SingleStrandCount.Should().Be(100);
            var stat = stepStats.OfType<SingleStrandCreationStat>().FirstOrDefault();
            stat.Should().NotBeNull();
            stat.AddedStrands.Should().Be(100);
        }
    }
}
