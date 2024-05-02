using System;

namespace TrulyRandom.Modules.Sources.Prng
{
    /// <summary>
    /// Generates data using a shift-register generator (Xorshift) PRNG algorithm. This source is unsecure and should never be used for cryptographical purposes, only for testing or debugging!
    /// </summary>
    [Obsolete("This source is unsecure and should be used only for testing or debugging purposes")]
    internal class XorshiftPrngSource : PrngSource
    {
        //Generator state
        ulong x;

        ///<inheritdoc/>
        public override int SeedSize => 4;

        ///<inheritdoc/>
        protected override void Initialize(byte[] seed)
        {
            while (seed[0] == 0 && seed[1] == 0 && seed[2] == 0 && seed[3] == 0)
            {
                seed = Utils.GetSystemRandom(4);
            }
            x = BitConverter.ToUInt32(seed);
        }

        ///<inheritdoc/>
        protected override byte[] NextBytes()
        {
            x ^= x << 13;
            x ^= x >> 17;
            x ^= x << 5;
            return BitConverter.GetBytes((uint)x);
        }
    }
}