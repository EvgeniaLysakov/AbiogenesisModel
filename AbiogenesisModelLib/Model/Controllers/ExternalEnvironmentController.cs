using AbiogenesisModel.Lib.Attributes;
using AbiogenesisModel.Lib.ConfigTypes.ControllerConfigTypes;
using AbiogenesisModel.Lib.Pipeline;

namespace AbiogenesisModel.Lib.Model.Controllers;

[Service]
public class ExternalEnvironmentController(IConfigFactory<ExternalEnvironmentControllerConfig> configFactory)
    : ConfigurableObject<ExternalEnvironmentControllerConfig>(configFactory)
{
    public ExternalEnvironment Create()
    {
        return new ExternalEnvironment(Configuration.InitialTemperature);
    }
}