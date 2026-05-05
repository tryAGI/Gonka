using System.Diagnostics;

namespace Gonka;

internal static class GonkaTimestamp
{
    private static readonly long WallClockBaseNanoseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1_000_000L;
    private static readonly long StopwatchBase = Stopwatch.GetTimestamp();

    public static long GetUnixTimeNanoseconds()
    {
        var elapsedTicks = Stopwatch.GetTimestamp() - StopwatchBase;
        var elapsedNanoseconds = elapsedTicks * 1_000_000_000L / Stopwatch.Frequency;
        return WallClockBaseNanoseconds + elapsedNanoseconds;
    }
}
