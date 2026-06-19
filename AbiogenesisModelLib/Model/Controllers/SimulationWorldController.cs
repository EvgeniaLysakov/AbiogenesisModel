using AbiogenesisModel.Lib.Attributes;
using AbiogenesisModel.Lib.ConfigTypes.ControllerConfigTypes;
using AbiogenesisModel.Lib.Model.DataTypes;
using AbiogenesisModel.Lib.Pipeline;

namespace AbiogenesisModel.Lib.Model.Controllers;

[Service]
public class SimulationWorldController(IConfigFactory<SimulationWorldControllerConfig> configFactory, ExternalEnvironmentController externalEnvironmentController, PondController pondController)
    : ConfigurableMultipleCreator<SimulationWorld, SimulationWorldControllerConfig>(configFactory)
{
    public override SimulationWorld Create()
    {
        return new SimulationWorld(externalEnvironmentController.Create(), pondController.CreateMany(Configuration.PondsNum));
    }
}