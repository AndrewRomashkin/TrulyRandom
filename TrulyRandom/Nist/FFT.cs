using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

// Source: Maht.NET
namespace TrulyRandom.Nist
{
    static class FFT
    {
        /// <summary>
        /// Applies Bluestein's FFT algorithm to the array
        /// </summary>
        /// <param name="samples">Data to be converted to a frequency domain</param>
        /// <param name="threads">Maximum threads to be utilized</param>
        public static void Apply(Complex[] samples, int threads)
        {
            DateTime lastBreak = DateTime.Now;
            Complex[] sequence = BluesteinSequence(samples.Length);
            Utils.BreakExecution(ref lastBreak);

            // Padding to power of two >= 2N–1 so we can apply Radix-2 FFT.
            int m = ((samples.Length << 1) - 1).CeilingToPowerOfTwo();
            Complex[] b = new Complex[m];
            Complex[] a = new Complex[m];

            Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = threads  },
                () =>
                {
                    Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                    DateTime lastBreak1 = DateTime.Now;
                    // Build and transform padded sequence b_k = exp(I*Pi*k^2/N)
                    for (int i = 0; i < samples.Length; i++)
                    {
                        b[i] = sequence[i];
                    }
                    Utils.BreakExecution(ref lastBreak1);

                    for (int i = m - samples.Length + 1; i < b.Length; i++)
                    {
                        b[i] = sequence[m - i];
                    }
                    Utils.BreakExecution(ref lastBreak1);

                    Radix2Forward(b);
                },
                () =>
                {
                    Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                    DateTime lastBreak1 = DateTime.Now;
                    // Build and transform padded sequence a_k = x_k * exp(-I*Pi*k^2/N)
                    for (int i = 0; i < samples.Length; i++)
                    {
                        a[i] = Complex.Conjugate(sequence[i]) * samples[i];
                        Utils.BreakExecution(ref lastBreak1);
                    }

                    Radix2Forward(a);
                });

            for (int i = 0; i < a.Length; i++)
            {
                a[i] *= b[i];
            }
            Utils.BreakExecution(ref lastBreak);

            Radix2InverseParallel(a, threads);

            double nbinv = 1.0 / m;
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = nbinv * Complex.Conjugate(sequence[i]) * a[i];
                Utils.BreakExecution(ref lastBreak);
            }
        }


        /// <summary>
        /// Generate the bluestein sequence for the provided problem size
        /// </summary>
        /// <param name="n">Number of samples</param>
        /// <returns>Bluestein sequence exp(I*Pi*k^2/N)</returns>
        static Complex[] BluesteinSequence(int n)
        {
            double s = Math.PI / n;
            Complex[] sequence = new Complex[n];

            for (int k = 0; k < sequence.Length; k++)
            {
                double t = s * k * k;
                sequence[k] = new Complex(Math.Cos(t), Math.Sin(t));
            }

            return sequence;
        }

        /// <summary>
        /// Find the closest perfect power of two that is larger or equal to the provided 32 bit integer
        /// </summary>
        /// <param name="number">The number of which to find the closest upper power of two</param>
        /// <returns>A power of two</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        static int CeilingToPowerOfTwo(this int number)
        {
            if (number == int.MinValue)
            {
                return 0;
            }

            const int maxPowerOfTwo = 0x40000000;
            if (number > maxPowerOfTwo)
            {
                throw new ArgumentOutOfRangeException(nameof(number));
            }

            number--;
            number |= number >> 1;
            number |= number >> 2;
            number |= number >> 4;
            number |= number >> 8;
            number |= number >> 16;
            return number + 1;
        }


        /// <summary>
        /// Radix-2 generic FFT for power-of-two sized sample vectors.
        /// </summary>
        static void Radix2Forward(Complex[] data)
        {
            DateTime lastBreak = DateTime.Now;
            Radix2Reorder(data);
            Utils.BreakExecution(ref lastBreak);
            for (int levelSize = 1; levelSize < data.Length; levelSize *= 2)
            {
                DateTime lastBreak1 = DateTime.Now;
                for (int k = 0; k < levelSize; k++)
                {
                    Radix2Step(data, -1, levelSize, k);
                    Utils.BreakExecution(ref lastBreak1);
                }
            }
        }

        /// <summary>
        /// Radix-2 generic FFT for power-of-two sample vectors (Parallel Version).
        /// </summary>
        static void Radix2InverseParallel(Complex[] data, int threads)
        {
            DateTime lastBreak = DateTime.Now;
            Radix2Reorder(data);
            Utils.BreakExecution(ref lastBreak);
            for (int levelSize = 1; levelSize < data.Length; levelSize *= 2)
            {
                int size = levelSize;

                Utils.ParallelFor(0, size, 64, threads, (u, v) =>
                {
                    Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                    DateTime lastBreak1 = DateTime.Now;
                    for (int i = u; i < v; i++)
                    {
                        Radix2Step(data, 1, size, i);
                        Utils.BreakExecution(ref lastBreak1);
                    }
                });
            }
        }

        /// <summary>
        /// Radix-2 Reorder Helper Method
        /// </summary>
        /// <typeparam name="T">Sample type</typeparam>
        /// <param name="samples">Sample vector</param>
        static void Radix2Reorder<T>(T[] samples)
        {
            int j = 0;
            for (int i = 0; i < samples.Length - 1; i++)
            {
                if (i < j)
                {
                    (samples[j], samples[i]) = (samples[i], samples[j]);
                }

                int m = samples.Length;

                do
                {
                    m >>= 1;
                    j ^= m;
                }
                while ((j & m) == 0);
            }
        }


        /// <summary>
        /// Radix-2 Step helper method
        /// </summary>
        /// <param name="samples">Sample vector.</param>
        /// <param name="exponentSign">Fourier series exponent sign.</param>
        /// <param name="levelSize">Level Group Size.</param>
        /// <param name="k">Index inside of the level.</param>
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        static void Radix2Step(Complex[] samples, int exponentSign, int levelSize, int k)
        {
            // Twiddle Factor
            double exponent = exponentSign * k * Math.PI / levelSize;
            Complex w = new((float)Math.Cos(exponent), (float)Math.Sin(exponent));

            int step = levelSize << 1;
            for (int i = k; i < samples.Length; i += step)
            {
                Complex ai = samples[i];
                Complex t = w * samples[i + levelSize];
                samples[i] = ai + t;
                samples[i + levelSize] = ai - t;
            }
        }
    }
}
