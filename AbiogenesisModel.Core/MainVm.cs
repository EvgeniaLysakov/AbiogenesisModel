using AbiogenesisModel.Lib;
using System.Collections.ObjectModel;

namespace AbiogenesisModel.Core
{
    public sealed class MainVm
    {
        private readonly AbiogenesisSite _site = new AbiogenesisSite();

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

        public void LoopAndBars()
        {
            var stats = _site.Loop();

            Feed.Add(new BarPlotItem(stats.SelectMany(stat => stat.ToBarPlotData()).Concat([
                ("Total nucleotides",_site.Pond.NucleotideCount),
                ("Free nucleotides",_site.Pond.FreeNucleotides.Length),
                ("Total single strands",_site.Pond.SingleStrandCount),
                ("Free single strands",_site.Pond.FreeStrands.Length),
                ("Total multi strands",_site.Pond.MultiStrandCount)
            ]).Concat(_site.Pond.AllStrands.GroupBy(strand => strand.Count).Select(group => ($"SingleStrand#{group.Key}", (double)group.Count()))).ToArray()));
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
