using System;
using System.Collections.Generic;
using System.Linq;
using TrulyRandom.Models;

namespace TrulyRandom
{
    /// <summary>
    /// Provides method for end-user to retrieve random data of varios types 
    /// </summary>
    public class DataSource
    {
        public int BitsAvailable => source.BytesInBuffer * 8 + buffer.Count;

        List<bool> buffer = new List<bool>();
        Module source;

        internal DataSource(Module source)
        {
            this.source = source;
        }

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

        public byte[] GetBytes(int count)
        {
            byte[] data = source.ReadExactly(count);
            if (data.Length != count)
            {
                throw new OutOfRandomnessException("Not enough randomness in the buffer");
            }
            return data;
        }

        public byte GetByte()
        {
            byte[] data = source.ReadExactly(1);
            if (data.Length != 1)
            {
                throw new OutOfRandomnessException("Not enough randomness in the buffer");
            }
            return data[0];
        }

        public ulong GetULong()
        {
            return GetULong(ulong.MaxValue);
        }

        public ulong GetULong(ulong maxValue)
        {
            //fast dice roller (FDR) algorithm
            lock (buffer)
            {
                ulong v = 1, c = 0;
                while (true)
                {
                    v = v << 1;
                    c = c << 1;
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

        public ulong GetULong(ulong minValue, ulong maxValue)
        {
            if (minValue >= maxValue)
            {
                throw new ArgumentException("Min value must be less than max value");
            }
            return GetULong(maxValue - minValue) + minValue;
        }

        public long GetLong()
        {
            return (long)GetULong(long.MaxValue);
        }

        public long GetLong(long maxValue)
        {
            if (maxValue <= 0)
            {
                throw new ArgumentException("Max value must be > 0");
            }
            return (long)GetULong((ulong)maxValue);
        }

        public long GetLong(long minValue, long maxValue)
        {
            if (minValue >= maxValue)
            {
                throw new ArgumentException("Min value must be less than max value");
            }
            return (long)GetULong((ulong)(maxValue - minValue)) + minValue;
        }

        public uint GetUInt()
        {
            return GetUInt(uint.MaxValue);
        }

        public uint GetUInt(uint maxValue)
        {
            lock (buffer)
            {
                uint v = 1, c = 0;
                while (true)
                {
                    v = v << 1;
                    c = c << 1;
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

        public uint GetUInt(uint minValue, uint maxValue)
        {
            if (minValue >= maxValue)
            {
                throw new ArgumentException("Min value must be less than max value");
            }
            return GetUInt(maxValue - minValue) + minValue;
        }

        public int GetInt()
        {
            return (int)GetUInt(int.MaxValue);
        }

        public int GetInt(int maxValue)
        {
            if (maxValue <= 0)
            {
                throw new ArgumentException("Max value must be > 0");
            }
            return (int)GetUInt((uint)maxValue);
        }

        public int GetInt(int minValue, int maxValue)
        {
            if (minValue >= maxValue)
            {
                throw new ArgumentException("Min value must be less than max value");
            }
            return (int)GetUInt((uint)(maxValue - minValue)) + minValue;
        }

        public void Shuffle<T>(T[] array)
        {
            uint n = (uint)array.Length;
            for (uint i = 0; i < (n - 1); i++)
            {
                uint r = i + GetUInt(n - i);
                T t = array[r];
                array[r] = array[i];
                array[i] = t;
            }
        }
    }
}
