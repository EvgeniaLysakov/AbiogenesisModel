using AbiogenesisModel.Lib;
using AbiogenesisModel.Lib.Pipeline;
using AbiogenesisModel.Lib.Steps.NucleotideCreators;
using AbiogenesisModel.Lib.Steps.SingleStrandCreators;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using AbiogenesisModel.Lib.Steps;

namespace AbiogenesisModel.Core
{
    public sealed class MainVm
    {
        private readonly ServiceProvider _provider;

        public MainVm()
        {
            var services = new ServiceCollection();
            services.RegisterDefaultGeneralConfig();
            services.RegisterConfigs();
            services.RegisterServices();

            _provider = services.BuildServiceProvider();
        }

        private int _feedLimit;

        public ObservableCollection<IStreamItem> Feed { get; } = [];

        public int FeedLimit
        {
            get => _feedLimit;
            set
            {
                if (value < 0)
                {
                    value = 0;
                }

                _feedLimit = value;
                LimitFeed();
            }
        }

        public void Run()
        {
            var vals = new Dictionary<int, double[]>();
            var lowerLimit = 10;
            var count = 100;
            for (var i = lowerLimit; i < lowerLimit + count; i++)
            {
                var site = _provider.GetRequiredService<AbiogenesisSite>();
                site.Pond.Clear();

                var j = i;
                var nucConfig = ((MultinomialNucleotideCreator)site.AbiogenesisCycle.NucleotideCreator).Configuration;
                var strandConfig = ((GrowingSingleStrandCreator)site.AbiogenesisCycle.SingleStrandCreator).Configuration;
                nucConfig.MaxNucleotidesInPond = 10000;
                nucConfig.MaxAddedNucleotides = i;
                strandConfig.MaxLigationEvents = null;
                strandConfig.NucleotidesToConsume = null;
                strandConfig.MaxAddedStrands = 10;
                StepStat[] stat;
                do
                {
                    stat = site.Loop();
                }
                while (/*site.Pond.FreeNucleotides.Any()*/ stat.OfType<NucleotideCreationStat>().First().AddedNucleotides > 0);

                var strandCounts = site.Pond.AllStrands.Select(strand => strand.Count).ToArray();
                vals.Add(j, [strandCounts.Average(), strandCounts.Max()]);
            }

            var keyPairs = vals.OrderBy(kv => kv.Key).ToArray();

            Feed.Add(new LinePlotItem([
                new LineData(keyPairs.Select(pair => ((double)pair.Key, pair.Value[0])).ToArray(), "Average length"),
                new LineData(keyPairs.Select(pair => ((double)pair.Key, pair.Value[1])).ToArray(), "Max length")]));

            LimitFeed();
        }

        public void Clear()
        {
            Feed.Clear();
        }

        private void LimitFeed()
        {
            if (_feedLimit == 0)
            {
                return;
            }

            while (Feed.Count > _feedLimit)
            {
                Feed.RemoveAt(0);
            }
        }
    }
}
