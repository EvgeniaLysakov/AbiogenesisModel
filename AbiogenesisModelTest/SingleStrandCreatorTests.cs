using AbiogenesisModel.Lib;
using AbiogenesisModel.Lib.Steps;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace AbiogenesisModel.Test
{
    public class SingleStrandCreatorTests : BaseTest
    {
        [Fact]
        public void NucleotideCount()
        {
            var provider = InitServiceCollection();
            var site = provider.GetRequiredService<AbiogenesisSite>();
            site.Pond.AllNucleotides.Length.Should().Be(0);

            var stepStats = site.Loop();
            site.Pond.AllNucleotides.Length.Should().Be(100);
            site.Pond.FreeNucleotides.Length.Should().BeLessThan(100);
            site.Pond.SingleStrandCount.Should().Be(10);
            var stat = stepStats.OfType<SingleStrandCreationStat>().FirstOrDefault();
            stat.Should().NotBeNull();
            stat.AddedStrands.Should().Be(10);
        }
    }
}
