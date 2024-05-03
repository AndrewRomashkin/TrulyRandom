using System;

namespace TrulyRandom.Modules.Sources.Prng;

/// <summary>
/// Generates data using a linear congruential generator (LCG) PRNG algorithm. This source is unsecure and should never be used for cryptographical purposes, only for testing or debugging!
/// </summary>
[Obsolete("This source is unsecure and should be used only for testing or debugging purposes")]
internal class LcgPrngSource : PrngSource
{
    //Modulus
    private readonly ulong m = (ulong)Math.Pow(2, 32);
    //Multiplier
    private const ulong a = 1664525;
    //Increment
    private const ulong c = 1013904223;
    //State
    private ulong x;

    ///<inheritdoc/>
    public override int SeedSize => 4;

    ///<inheritdoc/>
    protected override void Initialize(byte[] seed)
    {
        x = BitConverter.ToUInt32(seed);
    }

    ///<inheritdoc/>
    protected override byte[] NextBytes()
    {
        x = ((a * x) + c) % m;
        return BitConverter.GetBytes((uint)x);
    }
}
