namespace AbiogenesisModel.Lib.Pipeline;

public class ConfigurableObject<T>
    where T : ICloneable
{
    protected ConfigurableObject(IConfigFactory<T> configFactory)
    {
        Configuration = (T)configFactory.Get().Clone();
        ValidationHelper.ValidateAndThrow(Configuration);
    }

    public T Configuration { get; }
}