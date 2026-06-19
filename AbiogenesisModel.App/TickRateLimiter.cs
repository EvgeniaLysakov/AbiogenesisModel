using AbiogenesisModel.Lib.Guard;
using System.Diagnostics;

namespace AbiogenesisModel.App;

public sealed class TickRateLimiter
{
    private readonly TimeSpan _minTickInterval;
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    private long _nextTickTimestamp;

    public TickRateLimiter(double maxTicksPerSecond)
    {
        Ensure.That(maxTicksPerSecond).IsGreaterThan(0);

        _minTickInterval = TimeSpan.FromSeconds(1.0 / maxTicksPerSecond);
        _nextTickTimestamp = _stopwatch.ElapsedTicks;
    }

    public async Task WaitIfNeededAsync(CancellationToken cancellationToken)
    {
        _nextTickTimestamp += ToStopwatchTicks(_minTickInterval);

        var now = _stopwatch.ElapsedTicks;
        var delayTicks = _nextTickTimestamp - now;

        if (delayTicks > 0)
        {
            var delay = TimeSpan.FromSeconds((double)delayTicks / Stopwatch.Frequency);

            if (delay > TimeSpan.Zero)
            {
                Debug.WriteLine($"Delay {delay.TotalMilliseconds} ms");
                await Task.Delay(delay, cancellationToken);
                return;
            }
        }

        await Task.Delay(1, cancellationToken);
    }

    private static long ToStopwatchTicks(TimeSpan timeSpan)
    {
        return (long)(timeSpan.TotalSeconds * Stopwatch.Frequency);
    }
}