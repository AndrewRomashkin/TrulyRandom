using System;

namespace TrulyRandom
{
    /// <summary>
    /// Global constants
    /// </summary>
    static class Constants
    {
        internal static readonly TimeSpan AvgBytesPerSecondInterval = TimeSpan.FromMinutes(1);
        internal static readonly TimeSpan EnrtopyCalculationPeriod = TimeSpan.FromSeconds(1);
        internal const int EnrtopyCalculationBytes = 10000;
        internal const int CompressionCalculationBytes = 1000000;
    }
}
