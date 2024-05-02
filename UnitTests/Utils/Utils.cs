using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace UnitTests
{
    internal static partial class Utils
    {
        internal static BitArray ToBitArray(this string data)
        {
            List<bool> result = new();

            foreach (char ch in data)
            {
                if (ch == '1')
                {
                    result.Add(true);
                }
                else if (ch == '0')
                {
                    result.Add(false);
                }
            }

            return new BitArray(result.ToArray());
        }

        internal static byte[] ToByteArray(this string data)
        {
            byte[] result = new byte[data.Length / 8];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Convert.ToByte(data.Substring(i * 8, 8), 2);
            }

            return result;
        }

        internal static BitArray ToBitArray(this int[] data)
        {
            List<bool> result = new();

            foreach (int bit in data)
            {
                if (bit == 1)
                {
                    result.Add(true);
                }
                else if (bit == 0)
                {
                    result.Add(false);
                }
            }

            return new BitArray(result.ToArray());
        }

        internal static int[] ESpigot(int count, int @base)
        {
            int[] A = new int[count];
            int[] result = new int[count];

            for (int i = 0; i < count; i++)
            {
                A[i] = 1;
            }

            for (int i = 0; i < count; i++)
            {
                int q = 0;
                for (int j = count - 1; j >= 0; j--)
                {
                    A[j] = @base * A[j] + q;
                    q = A[j] / (j + 2);
                    A[j] = A[j] % (j + 2);
                }
                result[i] = q;
            }

            return result;
        }

        internal static bool WasAgo(this DateTime time, TimeSpan interval)
        {
            return time + interval < DateTime.Now;
        }

        internal static byte[] GetPseudorandomBytes(int count)
        {
            Random random = new();
            byte[] result = new byte[count];
            random.NextBytes(result);
            return result;
        }

        internal static void InvokePrivate(object obj, string methodName, params object[] parameters)
        {
            obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(obj, parameters);
        }

        internal static void AssignPrivate(object obj, string fieldName, object value)
        {
            obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(obj, value);
        }
    }
}