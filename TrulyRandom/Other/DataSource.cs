using System;
using System.Collections.Generic;
using System.Linq;
using TrulyRandom.Models;

namespace TrulyRandom
{
    /// <summary>
    /// Provides method for end-user to retrieve random data of various types 
    /// </summary>
    public class DataSource
    {
        /// <summary>
        /// Number if bits currently available in the buffer
        /// </summary>
        public int BitsAvailable => source.BytesInBuffer * 8 + buffer.Count;

        readonly List<bool> buffer = new();
        readonly Module source;

        internal DataSource(Module source)
        {
            this.source = source;
        }

        /// <summary>
        /// Gets a single random bit
        /// </summary>
        /// <returns>Random bit</returns>
        /// <exception cref="OutOfRandomnessException">Thrown if there is not enough data in the buffer</exception>
        public bool GetBit()
        {
            lock (buffer)
            {
                if (buffer.Any())
                {
                    bool result = buffer.Last();
                    buffer.RemoveAt(buffer.Count - 1);
                    return result;
                }

                byte[] data = source.ReadExactly(1);
                if (data.Length == 0)
                {
                    throw new OutOfRandomnessException("Not enough randomness in the buffer");
                }

                for (int i = 0; i < 7; i++)
                {
                    buffer.Add(data[0].Bit(i));
                }
                return data[0].Bit(7);
            }
        }

        /// <summary>
        /// Gets a single random byte
        /// </summary>
        /// <returns>Random byte</returns>
        /// <exception cref="OutOfRandomnessException">Thrown if there is not enough data in the buffer</exception>
        public byte GetByte()
        {
            byte[] data = source.ReadExactly(1);
            if (data.Length != 1)
            {
                throw new OutOfRandomnessException("Not enough randomness in the buffer");
            }
            return data[0];
        }

        /// <summary>
        /// Gets an array of random bytes
        /// </summary>
        /// <param name="count">Number of bytes</param>
        /// <returns>Random bytes</returns>
        /// <exception cref="OutOfRandomnessException">Thrown if there is not enough data in the buffer</exception>
        public byte[] GetBytes(int count)
        {
            byte[] data = source.ReadExactly(count);
            if (data.Length != count)
            {
                throw new OutOfRandomnessException("Not enough randomness in the buffer");
            }
            return data;
        }

        /// <summary>
        /// Gets a random ULong
        /// </summary>
        /// <returns>Random ULong</returns>
        /// <exception cref="OutOfRandomnessException">Thrown if there is not enough data in the buffer</exception>
        public ulong GetULong()
        {
            return GetULong(ulong.MaxValue);
        }

        /// <summary>
        /// Gets a random ULong in a range between 0 and <paramref name="maxValue"/>
        /// </summary>
        /// <param name="maxValue">Upper bound (not inclusive)</param>
        /// <returns>Random ULong</returns>
        /// <exception cref="OutOfRandomnessException">Thrown if there is not enough data in the buffer</exception>
        public ulong GetULong(ulong maxValue)
        {
            //Using FDR (Fast Dice Roller) algorithm
            lock (buffer)
            {
                ulong v = 1, c = 0;
                while (true)
                {
                    v <<= 1;
                    c <<= 1;
                    if (GetBit())
                    {
                        c++;
                    }
                    if (v >= maxValue)
                    {
                        if (c < maxValue)
                        {
                            return c;
                        }
                        v -= maxValue;
                        c -= maxValue;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a random ULong in a range between <paramref name="minValue"/> and <paramref name="maxValue"/>
        /// </summary>
        /// <param name="minValue">Lower bound (inclusive)</param>
        /// <param name="maxValue">Upper bound (not inclusive)</param>
        /// <returns>Random ULong</returns>
        /// <exception cref="OutOfRandomnessException">Thrown if there is not enough data in the buffer</exception>
        /// <exception cref="ArgumentException">Thrown if bounds are specified incorrectly</exception>
        public ulong GetULong(ulong minValue, ulong maxValue)
        {
            if (minValue >= maxValue)
            {
                throw new ArgumentException("Min value must be less than max value");
            }
            return GetULong(maxValue - minValue) + minValue;
        }

        /// <summary>
        /// Gets a random long
        /// </summary>
        /// <returns>Random long</returns>
        /// <exception cref="OutOfRandomnessException">Thrown if there is not enough data in the buffer</exception>
        public long GetLong()
        {
            return (long)GetULong(long.MaxValue);
        }

        /// <summary>
        /// Gets a random long in a range between 0 and <paramref name="maxValue"/>
        /// </summary>
        /// <param name="maxValue">Upper bound (not inclusive)</param>
        /// <returns>Random long</returns>
        /// <exception cref="OutOfRandomnessException">Thrown if there is not enough data in the buffer</exception>
        /// <exception cref="ArgumentException">Thrown if bound is specified incorrectly</exception>
        public long GetLong(long maxValue)
        {
            if (maxValue <= 0)
            {
                throw new ArgumentException("Max value must be > 0");
            }
            return (long)GetULong((ulong)maxValue);
        }

        /// <summary>
        /// Gets a random long in a range between <paramref name="minValue"/> and <paramref name="maxValue"/>
        /// </summary>
        /// <param name="minValue">Lower bound (inclusive)</param>
        /// <param name="maxValue">Upper bound (not inclusive)</param>
        /// <returns>Random long</returns>
        /// <exception cref="OutOfRandomnessException">Thrown if there is not enough data in the buffer</exception>
        /// <exception cref="ArgumentException">Thrown if bounds are specified incorrectly</exception>
        public long GetLong(long minValue, long maxValue)
        {
            if (minValue >= maxValue)
            {
                throw new ArgumentException("Min value must be less than max value");
            }
            return (long)GetULong((ulong)(maxValue - minValue)) + minValue;
        }

        /// <summary>
        /// Gets a random UInt
        /// </summary>
        /// <returns>Random UInt</returns>
        /// <exception cref="OutOfRandomnessException">Thrown if there is not enough data in the buffer</exception>
        public uint GetUInt()
        {
            return GetUInt(uint.MaxValue);
        }

        /// <summary>
        /// Gets a random UInt in a range between 0 and <paramref name="maxValue"/>
        /// </summary>
        /// <param name="maxValue">Upper bound (not inclusive)</param>
        /// <returns>Random UInt</returns>
        /// <exception cref="OutOfRandomnessException">Thrown if there is not enough data in the buffer</exception>
        public uint GetUInt(uint maxValue)
        {
            lock (buffer)
            {
                uint v = 1, c = 0;
                while (true)
                {
                    v <<= 1;
                    c <<= 1;
                    if (GetBit())
                    {
                        c++;
                    }
                    if (v >= maxValue)
                    {
                        if (c < maxValue)
                        {
                            return c;
                        }
                        v -= maxValue;
                        c -= maxValue;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a random UInt in a range between <paramref name="minValue"/> and <paramref name="maxValue"/>
        /// </summary>
        /// <param name="minValue">Lower bound (inclusive)</param>
        /// <param name="maxValue">Upper bound (not inclusive)</param>
        /// <returns>Random UInt</returns>
        /// <exception cref="OutOfRandomnessException">Thrown if there is not enough data in the buffer</exception>
        /// <exception cref="ArgumentException">Thrown if bounds are specified incorrectly</exception>
        public uint GetUInt(uint minValue, uint maxValue)
        {
            if (minValue >= maxValue)
            {
                throw new ArgumentException("Min value must be less than max value");
            }
            return GetUInt(maxValue - minValue) + minValue;
        }

        /// <summary>
        /// Gets a random int
        /// </summary>
        /// <returns>Random int</returns>
        /// <exception cref="OutOfRandomnessException">Thrown if there is not enough data in the buffer</exception>
        public int GetInt()
        {
            return (int)GetUInt(int.MaxValue);
        }

        /// <summary>
        /// Gets a random int in a range between 0 and <paramref name="maxValue"/>
        /// </summary>
        /// <param name="maxValue">Upper bound (not inclusive)</param>
        /// <returns>Random int</returns>
        /// <exception cref="OutOfRandomnessException">Thrown if there is not enough data in the buffer</exception>
        /// <exception cref="ArgumentException">Thrown if bound is specified incorrectly</exception>
        public int GetInt(int maxValue)
        {
            if (maxValue <= 0)
            {
                throw new ArgumentException("Max value must be > 0");
            }
            return (int)GetUInt((uint)maxValue);
        }

        /// <summary>
        /// Gets a random int in a range between <paramref name="minValue"/> and <paramref name="maxValue"/>
        /// </summary>
        /// <param name="minValue">Lower bound (inclusive)</param>
        /// <param name="maxValue">Upper bound (not inclusive)</param>
        /// <returns>Random int</returns>
        /// <exception cref="OutOfRandomnessException">Thrown if there is not enough data in the buffer</exception>
        /// <exception cref="ArgumentException">Thrown if bounds are specified incorrectly</exception>
        public int GetInt(int minValue, int maxValue)
        {
            if (minValue >= maxValue)
            {
                throw new ArgumentException("Min value must be less than max value");
            }
            return (int)GetUInt((uint)(maxValue - minValue)) + minValue;
        }


        /// <summary>
        /// Gets a random double in a range between 0 and 1
        /// </summary>
        /// <param name="including0">Specifies whether 0 should be incuded</param>
        /// <param name="including1">Specifies whether 1 should be incuded</param>
        /// <returns>Random double</returns>
        /// <exception cref="OutOfRandomnessException">Thrown if there is not enough data in the buffer</exception>
        public double GetDouble(bool including0 = true, bool including1 = false)
        {
            return GetInt(including0 ? 0 : 1, including1 ? int.MaxValue : int.MaxValue - 1) * (1.0 / int.MaxValue);
        }

        /// <summary>
        /// Gets a random double in a range between 0 and 1 (inclusive)
        /// </summary>
        /// <param name="minValue">Lower bound</param>
        /// <param name="maxValue">Upper bound</param>
        /// <param name="includingMin">Specifies whether minValue should be incuded</param>
        /// <param name="includingMax">Specifies whether maxValue should be incuded</param>
        /// <returns>Random double</returns>
        /// <exception cref="OutOfRandomnessException">Thrown if there is not enough data in the buffer</exception>
        public double GetDouble(double minValue, double maxValue, bool includingMin = true, bool includingMax = false)
        {
            return GetDouble(includingMin, includingMax) * (maxValue - minValue) + minValue;
        }

        /// <summary>
        /// Gets a random double that follows Gaussian (normal) distribution
        /// </summary>
        /// <param name="mean">Mean value</param>
        /// <param name="stdDev">Standard deviation</param>
        /// <returns></returns>
        public double GetNormal(double mean, double stdDev)
        {
            double u1 = 1.0 - GetDouble(true, false);
            double u2 = 1.0 - GetDouble(true, false);
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + stdDev * randStdNormal;
        }

        /// <summary>
        /// Randomly shuffles specified array using Fisher–Yates algorithm
        /// </summary>
        /// <typeparam name="T">Type of an array element</typeparam>
        /// <param name="array">Array to be shuffled</param>
        public void Shuffle<T>(T[] array)
        {
            uint n = (uint)array.Length;
            for (uint i = 0; i < (n - 1); i++)
            {
                uint r = i + GetUInt(n - i);
                (array[i], array[r]) = (array[r], array[i]);
            }
        }
    }
}
