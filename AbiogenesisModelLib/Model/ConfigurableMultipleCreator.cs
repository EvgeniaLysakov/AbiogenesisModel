using AbiogenesisModel.Lib.Pipeline;

namespace AbiogenesisModel.Lib.Model;

public abstract class ConfigurableMultipleCreator<TDataType, TConfig>(IConfigFactory<TConfig> configFactory)
    : ConfigurableObject<TConfig>(configFactory)
    where TConfig : ICloneable
{
    public abstract TDataType Create();

    public IReadOnlyList<TDataType> CreateMany(int num)
    {
        var result = new List<TDataType>();
        for (var i = 0; i < num; i++)
        {
            result.Add(Create());
        }

        return result;
    }
}