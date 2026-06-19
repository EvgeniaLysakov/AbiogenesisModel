using AbiogenesisModel.Lib.Attributes;
using AbiogenesisModel.Lib.ConfigTypes.ControllerConfigTypes;
using AbiogenesisModel.Lib.Pipeline;

namespace AbiogenesisModel.Lib.Model.Controllers;

[Service]
public class PondController(IConfigFactory<PondControllerConfig> configFactory, StratumController stratumController)
    : ConfigurableMultipleCreator<Pond, PondControllerConfig>(configFactory)
{
    public override Pond Create()
    {
        return new Pond(stratumController.CreateMany(Configuration.StrataNum));
    }
}