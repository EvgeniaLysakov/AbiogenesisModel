using AbiogenesisModel.Lib.Configurability;
using AbiogenesisModel.Lib.DataTypes;
using AbiogenesisModel.Lib.Extensions;
using Microsoft.Extensions.Configuration;

namespace AbiogenesisModel.Lib.Steps
{
    [ConfigurableStep("dummy", IsDefault = true)]
    public class DummySingleStrandCreator : ISingleStrandCreator
    {
        public void ValidateAndInit()
        {
        }

        public SingleStrandCreationStat Create(SingleStrandCreationLimit limit, Pond pond)
        {
            return new SingleStrandCreationStat(0, 0, 0);
        }
    }

    [ConfigurableStep("Multinomial")]
    public class MultinomialSingleStrandCreator(IConfiguration config) : ConfigurableObject(config), ISingleStrandCreator
    {
        private readonly Random _random = new();

        public void ValidateAndInit()
        {
            if (Probabilities.Keys.Any(key => key <= 1))
            {
                throw new InvalidOperationException("SingleStrand creator must create strands with at least 2 nucleotides");
            }

            if (Probabilities.Values.Any(val => val < 0))
            {
                throw new InvalidOperationException("Probability can't be less than 0");
            }

            var keys = Probabilities.Keys.ToArray();
            foreach (var key in keys)
            {
                if (Probabilities[key] == 0)
                {
                    Probabilities.Remove(key);
                }
            }

            var total = Probabilities.Values.Sum();
            keys = Probabilities.Keys.ToArray();

            foreach (var key in keys)
            {
                Probabilities[key] /= total;
            }
        }

        [Configurable("Probabilities")]
        public Dictionary<int, double> Probabilities { get; internal set; } = new();

        public SingleStrandCreationStat Create(SingleStrandCreationLimit limit, Pond pond)
        {
            var freeNucleotides = pond.FreeNucleotides.ToList();

            var consumedNucleotides = 0;
            var ligationEvents = 0;
            var addedStrands = 0;

            while (!limit.IsReached(consumedNucleotides, ligationEvents, addedStrands))
            {
                var len = GetRandomLen();

                var nucs = new List<Nucleotide>();

                for (var j = 0; j < len && freeNucleotides.Any(); j++)
                {
                    var nuc = _random.Choice(freeNucleotides);
                    freeNucleotides.Remove(nuc);
                    nucs.Add(nuc);
                }

                if (nucs.Count <= 1)
                {
                    break;
                }

                pond.Add(new SingleStrand(nucs));
                addedStrands++;
                consumedNucleotides += nucs.Count;
                ligationEvents += nucs.Count - 1;
            }

            return new SingleStrandCreationStat(consumedNucleotides, ligationEvents, addedStrands);
        }

        private int GetRandomLen()
        {
            var rndVal = _random.NextDouble();

            foreach (var (key, value) in Probabilities)
            {
                rndVal -= value;
                if (rndVal <= 0)
                {
                    return key;
                }
            }

            return 0;
        }
    }

    [ConfigurableStep("growing")]
    public class GrowingSingleStrandCreator(IConfiguration config) : ConfigurableObject(config), ISingleStrandCreator
    {
        private readonly Random _random = new();

        public void ValidateAndInit()
        {
        }

        [Configurable("Internal")]
        public bool IsInternal { get; internal set; }

        public SingleStrandCreationStat Create(SingleStrandCreationLimit limit, Pond pond)
        {
            var freeNucleotides = pond.FreeNucleotides.ToList();
            var activeStrands = IsInternal ? pond.AllStrands.ToDictionary(strand => strand.Last()) : [];
            var activeNucleotides = freeNucleotides.Concat(activeStrands.Keys).ToList();

            var consumedNucleotides = 0;
            var ligationEvents = 0;
            var addedStrands = 0;

            while (!limit.IsReached(consumedNucleotides, ligationEvents, addedStrands))
            {
                if (activeNucleotides.Count == 0)
                {
                    break;
                }

                var nuc_3 = _random.Choice(activeNucleotides);
                freeNucleotides.Remove(nuc_3);
                activeNucleotides.Remove(nuc_3);

                if (freeNucleotides.Count == 0)
                {
                    break;
                }

                var nuc_5 = _random.Choice(freeNucleotides);
                freeNucleotides.Remove(nuc_5);

                if (!activeStrands.Remove(nuc_3, out var strand))
                {
                    strand = new SingleStrand(nuc_3);
                    pond.Add(strand);
                    consumedNucleotides++;
                    addedStrands++;
                }

                strand.Add(nuc_5);
                consumedNucleotides++;
                ligationEvents++;

                activeStrands.Add(nuc_5, strand);
            }

            return new SingleStrandCreationStat(consumedNucleotides, ligationEvents, addedStrands);
        }
    }
}
