using System;

namespace TrulyRandom
{
    static class Constants
    {
        public static readonly TimeSpan AvgBytesPerSecondInterval = TimeSpan.FromMinutes(1);
        public static readonly TimeSpan EnrtopyCalculationPeriod = TimeSpan.FromSeconds(1);
        public const int EnrtopyCalculationBytes = 10000;
        public const int CompressionCalculationBytes = 1000000;
    }
}
