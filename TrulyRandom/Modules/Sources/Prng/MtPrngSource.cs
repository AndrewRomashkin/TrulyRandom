using System;

namespace TrulyRandom.Modules.Sources.Prng
{
    /// <summary>
    /// Generates data using a Mersenne twister (MT) PRNG algorithm. This source is unsecure and should never be used for cryptographical purposes, only for testing or debugging!
    /// </summary>
    [Obsolete("This source is unsecure and should be used only for testing or debugging purposes")]
    internal class MtPrngSource : PrngSource
    {
        // Period parameters
        private const int n = 624;
        private const int m = 397;
        private const uint matrixA = 0x9908b0df;
        private const uint upperMask = 0x80000000;
        private const int lowerMask = 0x7fffffff;
        private const uint spectralTestMultiplier = 1812433253; // see Knuth TAOCP Vol2. 3rd Ed. P.106

        // Tempering bit masks
        private const uint b = 0x9d2c5680;
        private const uint c = 0xefc60000;

        // Private static variables
        private static readonly uint[] mag01 = { 0, matrixA };

        // Member variables
        private readonly uint[] state = new uint[n];
        private int index = n + 1;

        ///<inheritdoc/>
        public override int SeedSize => 4;

        ///<inheritdoc/>
        protected override byte[] NextBytes()
        {
            if (index >= n)
            {
                Twist();
            }

            return BitConverter.GetBytes(Temper(state[index++]));
        }

        ///<inheritdoc/>
        protected override void Initialize(byte[] seed)
        {
            state[0] = BitConverter.ToUInt32(seed);

            for (index = 1; index < n; index++)
            {
                state[index] = (uint)(spectralTestMultiplier * (state[index - 1] ^ (state[index - 1] >> 30)) + index);
            }
        }

        private void Twist()
        {
            uint twister;
            int twistIndex;

            for (twistIndex = 0; twistIndex < n - 1; twistIndex++)
            {
                twister = (state[twistIndex] & upperMask) | (state[twistIndex + 1] & lowerMask);
                state[twistIndex] = state[(twistIndex + m) % n] ^ (twister >> 1) ^ mag01[twister & 1];
            }

            twister = (state[n - 1] & upperMask) | (state[0] & lowerMask);
            state[n - 1] = state[m - 1] ^ (twister >> 1) ^ mag01[twister & 1];

            index = 0;
        }

        private static uint Temper(uint next)
        {
            next ^= next >> 11;
            next ^= (next << 7) & b;
            next ^= (next << 15) & c;
            next ^= next >> 18;

            return next;
        }
    }
}
