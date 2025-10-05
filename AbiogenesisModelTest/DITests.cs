using AbiogenesisModel.Lib;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace AbiogenesisModel.Test;

public class DITests : BaseTest
{
    [Fact]
    public void CreateTest()
    {
        var provider = InitServiceCollection();

        var site = provider.GetRequiredService<AbiogenesisSite>();
        site.Should().NotBeNull();
        site.AbiogenesisCycle.Should().NotBeNull();
        site.AbiogenesisCycle.NucleotideCreator.Should().NotBeNull();
        site.AbiogenesisCycle.SingleStrandCreator.Should().NotBeNull();
    }
}