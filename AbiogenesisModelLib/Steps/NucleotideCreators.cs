using AbiogenesisModel.Lib.Configurability;
using AbiogenesisModel.Lib.DataTypes;
using Microsoft.Extensions.Configuration;

// ReSharper disable UnusedMember.Global
namespace AbiogenesisModel.Lib.Steps
{
    [ConfigurableStep("dummy", IsDefault = true)]
    public class DummyNucleotideCreator : INucleotideCreator
    {
        public void ValidateAndInit()
        {
        }

        public NucleotideCreationStat Create(NucleotideCreationLimit limit, Pond pond)
        {
            return new NucleotideCreationStat(0);
        }
    }

    [ConfigurableStep("multinomial")]
    public class MultinomialNucleotideCreator(IConfiguration config) : ConfigurableObject(config), INucleotideCreator
    {
        private readonly Random _random = new();

        [Configurable("A")]
        public double AProbability { get; internal set; }

        [Configurable("U")]
        public double UProbability { get; internal set; }

        [Configurable("C")]
        public double CProbability { get; internal set; }

        [Configurable("G")]
        public double GProbability { get; internal set; }

        public void ValidateAndInit()
        {
            var totalProbability = AProbability + UProbability + CProbability + GProbability;
            if (totalProbability == 0)
            {
                throw new InvalidOperationException("The total probability can't be 0");
            }

            // normalize probabilities
            AProbability /= totalProbability;
            UProbability /= totalProbability;
            CProbability /= totalProbability;
            GProbability /= totalProbability;
        }

        private Nucleotide.Nucleobase GetRandomNucleobase()
        {
            var rndVal = _random.NextDouble();
            if (rndVal <= AProbability)
            {
                return Nucleotide.Nucleobase.A;
            }

            rndVal -= AProbability;
            if (rndVal <= UProbability)
            {
                return Nucleotide.Nucleobase.U;
            }

            rndVal -= UProbability;
            if (rndVal <= CProbability)
            {
                return Nucleotide.Nucleobase.C;
            }

            rndVal -= CProbability;
            if (rndVal <= GProbability)
            {
                return Nucleotide.Nucleobase.G;
            }

            throw new InvalidOperationException("Invalid random value");
        }

        public NucleotideCreationStat Create(NucleotideCreationLimit limit, Pond pond)
        {
            var addedNucleotides = 0;
            while (!limit.IsReached(addedNucleotides, pond.NucleotideCount))
            {
                var nucleobase = GetRandomNucleobase();

                pond.Add(new Nucleotide(nucleobase));
                addedNucleotides++;
            }

            return new NucleotideCreationStat(addedNucleotides);
        }
    }
}
