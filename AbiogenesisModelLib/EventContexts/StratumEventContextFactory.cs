using AbiogenesisModel.Lib.Attributes;
using AbiogenesisModel.Lib.Model;
using AbiogenesisModel.Lib.Model.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbiogenesisModel.Lib.EventContexts;

[Service]
public class StratumEventContextFactory(StratumController stratumController, StratumPopulationController stratumPopulationController)
{
    public StratumEventContext Create(ExternalEnvironment externalEnvironment, Stratum stratum)
    {
        return new StratumEventContext(externalEnvironment, stratum, stratumController, stratumPopulationController);
    }
}