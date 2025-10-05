using AbiogenesisModel.Lib.Pipeline;
using System.ComponentModel.DataAnnotations;

namespace AbiogenesisModel.Core.Runners
{
    [NamedService("limitRange")]
    public class LimitRangeRunner(IConfigFactory<LimitRangeRunnerConfig> configFactory) : ConfigurableObject<LimitRangeRunnerConfig>(configFactory), IRunner
    {
        public void Run()
        {
            throw new NotImplementedException();
        }
    }

    [Config("Runner\\LimitRange")]
    public class LimitRangeRunnerConfig : IValidatableObject, ICloneable
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int Start { get; init; } = 1;

        [Required]
        [Range(1, int.MaxValue)]
        public int Count { get; init; } = 100;

        [Range(1, int.MaxValue)]
        public int Repeats { get; init; } = 1;

        [Required]
        [MinLength(1)]
        public List<LimitRangeItem> Limits { get; init; } = [];

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Limits.GroupBy(limit => limit.Name).Any(group => group.Count() > 1))
            {
                yield return new ValidationResult("Each limit type can only be specified once");
            }
        }

        public object Clone()
        {
            return new LimitRangeRunnerConfig() { Start = Start, Count = Count, Limits = Limits };
        }
    }

    public enum LimitType
    {
        MaxAddedNucleotides,
        MaxNucleotidesInPond,
        MaxAddedStrands,
        MaxLigationEvents,
        NucleotidesToConsume
    }

    public record LimitRangeItem(LimitType Name, double Factor, double Offset);
}
