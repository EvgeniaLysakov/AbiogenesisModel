using AbiogenesisModel.Lib.Model.DataTypes;

namespace AbiogenesisModel.Telemetry.Interfaces;

public interface IStateStatisticsProvider
{
    ISimulationStatistic? Collect(SimulationWorld simulationWorld);
}