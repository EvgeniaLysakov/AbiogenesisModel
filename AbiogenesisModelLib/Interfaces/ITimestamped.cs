using AbiogenesisModel.Lib.Model;

namespace AbiogenesisModel.Lib.Interfaces;

public interface ITimestamped
{
    Timestamp Timestamp { get; }

    void TouchTimestamp();
}