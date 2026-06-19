using AbiogenesisModel.Lib.Attributes;
using AbiogenesisModel.Lib.ConfigTypes.ControllerConfigTypes;
using AbiogenesisModel.Lib.Pipeline;

namespace AbiogenesisModel.Lib.Model.Controllers;

[Service]
public class StratumController(IConfigFactory<StratumControllerConfig> configFactory, StratumPopulationController stratumPopulationController)
    : ConfigurableMultipleCreator<Stratum, StratumControllerConfig>(configFactory)
{
    public override Stratum Create()
    {
        return new Stratum(stratumPopulationController.Create(), stratumPopulationController.Create());
    }
}