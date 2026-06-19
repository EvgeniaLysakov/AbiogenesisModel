namespace AbiogenesisModel.Telemetry.Interfaces;

public interface ICachedStatisticsProvider
{
    ISimulationStatistic? Flush();

    void Reset();
}