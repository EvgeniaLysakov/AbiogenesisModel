using System.Diagnostics;

namespace AbiogenesisModel.Lib.DebugTools;

public sealed class TimeMeasurer(string message) : IDisposable
{
    private readonly string _message = message;
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    public void Dispose()
    {
        _stopwatch.Stop();
        Debug.WriteLine($"{_message}: {_stopwatch.Elapsed.TotalMilliseconds} ms");
    }
}