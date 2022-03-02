﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrulyRandom
{
    public static class Utils
    {
        static readonly string[] SizeSuffixes =
                  { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public static string FormatBytes(long value)
        {
            if (value < 0)
            {
                return "-" + FormatBytes((ulong)-value);
            }
            return FormatBytes((ulong)value);
        }

        public static string FormatBytes(ulong value)
        {
            int i = 0;
            decimal dValue = value;
            while (dValue >= 2048)
            {
                dValue /= 1024;
                i++;
            }

            return $"{dValue:n0} {SizeSuffixes[Math.Min(i, 8)]}";
        }

        internal static T[] Append<T>(this T[] array, T item)
        {
            List<T> list = array.ToList();
            list.Add(item);
            return list.ToArray();
        }

        internal static T[] Concat<T>(this T[] array1, T[] array2)
        {
            T[] result = new T[array1.Length + array2.Length];
            array1.CopyTo(result, 0);
            array2.CopyTo(result, array1.Length);
            return result;
        }

        internal static bool WasAgo(this DateTime time, TimeSpan interval)
        {
            return time + interval < DateTime.Now;
        }

        internal static bool WasAgo(this DateTime time, int interval)
        {
            return time + TimeSpan.FromMilliseconds(interval) < DateTime.Now;
        }

        internal static DateTime Max(this DateTime first, DateTime second)
        {
            if (first >= second)
            {
                return first;
            }
            return second;
        }

        internal static DateTime Min(this DateTime first, DateTime second)
        {
            if (first <= second)
            {
                return first;
            }
            return second;
        }

        internal static byte[] ToByteArray(this BitArray data)
        {
            if (data == null || data.Length < 8)
            {
                return new byte[0];
            }
            byte[] result = new byte[data.Length / 8];
            data.CopyTo(result, 0);
            return result;
        }

        internal static string ToBitString(this BitArray data)
        {
            if (data == null)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i] ? '1' : '0');
            }
            return sb.ToString();
        }

        internal static byte[] ToByteArray(this bool[] data)
        {
            if (data == null || data.Length < 8)
            {
                return new byte[0];
            }
            int bytes = data.Length / 8;
            byte[] result = new byte[bytes];
            int bitIndex = 0, byteIndex = 0;
            for (int i = 0; i < bytes * 8; i++)
            {
                if (data[i])
                {
                    result[byteIndex] |= (byte)(1 << bitIndex);
                }
                bitIndex++;
                if (bitIndex == 8)
                {
                    bitIndex = 0;
                    byteIndex++;
                }
            }
            return result;
        }

        internal static bool Bit(this byte bt, int index)
        {
            return (bt & (1 << index)) != 0;
        }

        internal static T[] Subarray<T>(this T[] data, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, 0, result, 0, length);
            return result;
        }

        internal static T[] Subarray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        internal static BitArray Append(this BitArray current, BitArray after)
        {
            bool[] bools = new bool[current.Count + after.Count];
            current.CopyTo(bools, 0);
            after.CopyTo(bools, current.Count);
            return new BitArray(bools);
        }

        internal static BitArray Append(this BitArray current, bool[] after)
        {
            bool[] bools = new bool[current.Count + after.Length];
            current.CopyTo(bools, 0);
            after.CopyTo(bools, current.Count);
            return new BitArray(bools);
        }

        internal static void Reverse(this BitArray array)
        {
            int length = array.Length;
            int mid = length / 2;

            for (int i = 0; i < mid; i++)
            {
                bool bit = array[i];
                array[i] = array[length - i - 1];
                array[length - i - 1] = bit;
            }
        }

        internal static IEnumerable<T> EnumerateFlags<T>(this T input) where T : Enum
        {
            foreach (Enum value in Enum.GetValues(input.GetType()))
            {
                if (input.HasFlag(value) && ((int)(object)value != 0) && (((int)(object)value & ((int)(object)value - 1)) == 0))
                {
                    yield return (T)value;
                }
            }
        }

        //mix the data from different sources
        internal static byte[] MixData(byte[][] data)
        {
            List<byte> result = new List<byte>(data[0]);

            for (int i = 1; i < data.Length; i++)
            {
                List<byte> mix = new List<byte>();

                int index1 = 0, index2 = 0;
                double ratio = result.Count / (double)data[i].Length;
                while (index1 < result.Count || index2 < data[i].Length)
                {
                    if (index1 < result.Count && index1 < index2 * ratio)
                    {
                        mix.Add(result[index1]);
                        index1++;
                    }
                    else
                    {
                        mix.Add(data[i][index2]);
                        index2++;
                    }
                }
                result = mix;
            }
            return result.ToArray();
        }

        /// <summary>
        /// Executes a for loop in which iterations may run in parallel.
        /// </summary>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="rangeSize">The partition size for splitting work into smaller pieces.</param>
        /// <param name="body">The body to be invoked for each iteration range.</param>
        public static void ParallelFor(int fromInclusive, int toExclusive, int rangeSize, int threads, Action<int, int> body)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            if (fromInclusive < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(fromInclusive));
            }

            if (fromInclusive > toExclusive)
            {
                throw new ArgumentOutOfRangeException(nameof(toExclusive));
            }

            if (rangeSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(rangeSize));
            }

            int length = toExclusive - fromInclusive;

            // Special case: nothing to do
            if (length <= 0)
            {
                return;
            }

            // Special case: not worth to parallelize, inline
            if (threads < 2 || (rangeSize * 2) > length)
            {
                body(fromInclusive, toExclusive);
                return;
            }

            // Common case
            Parallel.ForEach(
                System.Collections.Concurrent.Partitioner.Create(fromInclusive, toExclusive, rangeSize),
                new ParallelOptions() { MaxDegreeOfParallelism = threads },
                range => body(range.Item1, range.Item2));
        }

        internal static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N)
        {
            return source.Skip(Math.Max(0, source.Count() - N));
        }

        internal static string FormatTable(string[] rowHeaders, string[][] data, int columnWidth)
        {
            StringBuilder sb = new StringBuilder();

            for (int row = 0; row < rowHeaders.Length; row++)
            {
                sb.Append(rowHeaders[row].PadRight(rowHeaders.Max(x => x.Length) + 2));
                for (int col = 0; col < data[row].Length; col++)
                {
                    sb.Append(data[row][col].PadRight(columnWidth));
                }
                if (row != rowHeaders.Length - 1)
                {
                    sb.Append("\n");
                }
            }
            return sb.ToString();
        }

        internal static void BreakExecution(ref DateTime lastBreak)
        {
            if (lastBreak.WasAgo(100))
            {
                System.Threading.Thread.Sleep(1);
                lastBreak = DateTime.Now;
            }
        }
    }
}
