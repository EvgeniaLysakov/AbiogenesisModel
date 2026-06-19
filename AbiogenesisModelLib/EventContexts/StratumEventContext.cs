using AbiogenesisModel.Lib.Interfaces;
using AbiogenesisModel.Lib.Model;
using AbiogenesisModel.Lib.Model.Controllers;

namespace AbiogenesisModel.Lib.EventContexts;

public class StratumEventContext(ExternalEnvironment externalEnvironment, Stratum stratum, StratumController stratumController, StratumPopulationController stratumPopulationController) : IContext
{
    public ExternalEnvironment ExternalEnvironment { get; } = externalEnvironment;

    public Stratum Stratum { get; } = stratum;

    public StratumController StratumController { get; } = stratumController;

    public StratumPopulationController StratumPopulationController { get; } = stratumPopulationController;
}