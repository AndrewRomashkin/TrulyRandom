using System;
using System.Collections;

namespace TrulyRandom
{
    //Copyright(C) 1984 Stephen L.Moshier(original C version - Cephes Math Library)
    //Copyright(C) 2005 Miroslav Stampar(C# version)
    /// <summary>
    /// Additional functions required by the test functions
    /// </summary>
    static class NistTestUtils
    {
        const double MACHEP = 1.11022302462515654042E-16;
        const double MAXLOG = 7.09782712893383996732E2;
        const double LOGPI = 1.14472988584940017414;
        const double SQRTH = 7.07106781186547524401E-1;

        /// <summary>
        /// Complementary Gauss error function
        /// </summary>
        internal static double Erfc(double x)
        {
            // constants
            double a1 = 0.254829592;
            double a2 = -0.284496736;
            double a3 = 1.421413741;
            double a4 = -1.453152027;
            double a5 = 1.061405429;
            double p = 0.3275911;

            // Save the sign of x
            int sign = 1;
            if (x < 0)
            {
                sign = -1;
            }

            x = Math.Abs(x);

            // A&S formula 7.1.26
            double t = 1.0 / (1.0 + p * x);
            double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);

            return 1 - sign * y;
        }

        /// <summary>
        /// Complemented incomplete gamma function.
        /// </summary>
        internal static double Igamc(double a, double x)
        {
            double big = 4.503599627370496e15;
            double biginv = 2.22044604925031308085e-16;
            double ans, ax, c, yc, r, t, y, z;
            double pk, pkm1, pkm2, qk, qkm1, qkm2;

            if (x <= 0 || a <= 0)
            {
                return 1.0;
            }

            if (x < 1.0 || x < a)
            {
                return 1.0 - Igam(a, x);
            }

            ax = a * Math.Log(x) - x - Lgamma(a);
            if (ax < -MAXLOG)
            {
                return 0.0;
            }

            ax = Math.Exp(ax);

            /* continued fraction */
            y = 1.0 - a;
            z = x + y + 1.0;
            c = 0.0;
            pkm2 = 1.0;
            qkm2 = x;
            pkm1 = x + 1.0;
            qkm1 = z * x;
            ans = pkm1 / qkm1;

            do
            {
                c += 1.0;
                y += 1.0;
                z += 2.0;
                yc = y * c;
                pk = pkm1 * z - pkm2 * yc;
                qk = qkm1 * z - qkm2 * yc;
                if (qk != 0)
                {
                    r = pk / qk;
                    t = Math.Abs((ans - r) / r);
                    ans = r;
                }
                else
                {
                    t = 1.0;
                }

                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;
                if (Math.Abs(pk) > big)
                {
                    pkm2 *= biginv;
                    pkm1 *= biginv;
                    qkm2 *= biginv;
                    qkm1 *= biginv;
                }
            } while (t > MACHEP);

            return ans * ax;
        }

        /// <summary>
        /// Incomplete gamma function.
        /// </summary>
        static double Igam(double a, double x)
        {
            double ans, ax, c, r;

            if (x <= 0 || a <= 0)
            {
                return 0.0;
            }

            if (x > 1.0 && x > a)
            {
                return 1.0 - Igamc(a, x);
            }

            /* Compute  x**a * exp(-x) / gamma(a)  */
            ax = a * Math.Log(x) - x - Lgamma(a);
            if (ax < -MAXLOG)
            {
                return 0.0;
            }

            ax = Math.Exp(ax);

            /* power series */
            r = a;
            c = 1.0;
            ans = 1.0;

            do
            {
                r += 1.0;
                c *= x / r;
                ans += c;
            } while (c / ans > MACHEP);

            return (ans * ax / a);
        }

        /// <summary>
        /// Natural logarithm of gamma function.
        /// </summary>
        static double Lgamma(double x)
        {
            double p, q, w, z;

            double[] A =
                {
                    8.11614167470508450300E-4,
                    -5.95061904284301438324E-4,
                    7.93650340457716943945E-4,
                    -2.77777777730099687205E-3,
                    8.33333333333331927722E-2
                };
            double[] B =
                {
                    -1.37825152569120859100E3,
                    -3.88016315134637840924E4,
                    -3.31612992738871184744E5,
                    -1.16237097492762307383E6,
                    -1.72173700820839662146E6,
                    -8.53555664245765465627E5
                };
            double[] C =
                {
                    /* 1.00000000000000000000E0, */
                    -3.51815701436523470549E2,
                    -1.70642106651881159223E4,
                    -2.20528590553854454839E5,
                    -1.13933444367982507207E6,
                    -2.53252307177582951285E6,
                    -2.01889141433532773231E6
                };

            if (x < -34.0)
            {
                q = -x;
                w = Lgamma(q);
                p = Math.Floor(q);
                if (p == q)
                {
                    throw new ArithmeticException("lgam: Overflow");
                }

                z = q - p;
                if (z > 0.5)
                {
                    p += 1.0;
                    z = p - q;
                }
                z = q * Math.Sin(Math.PI * z);
                if (z == 0.0)
                {
                    throw new
                        ArithmeticException("lgamma: Overflow");
                }

                z = LOGPI - Math.Log(z) - w;
                return z;
            }

            if (x < 13.0)
            {
                z = 1.0;
                while (x >= 3.0)
                {
                    x -= 1.0;
                    z *= x;
                }
                while (x < 2.0)
                {
                    if (x == 0.0)
                    {
                        throw new
                            ArithmeticException("lgamma: Overflow");
                    }

                    z /= x;
                    x += 1.0;
                }
                if (z < 0.0)
                {
                    z = -z;
                }

                if (x == 2.0)
                {
                    return Math.Log(z);
                }

                x -= 2.0;
                p = x * Polevl(x, B, 5) / P1evl(x, C, 6);
                return (Math.Log(z) + p);
            }

            if (x > 2.556348e305)
            {
                throw new
                    ArithmeticException("lgamma: Overflow");
            }

            q = (x - 0.5) * Math.Log(x) - x + 0.91893853320467274178;
            if (x > 1.0e8)
            {
                return (q);
            }

            p = 1.0 / (x * x);
            if (x >= 1000.0)
            {
                q += ((7.9365079365079365079365e-4 * p
                       - 2.7777777777777777777778e-3) * p
                      + 0.0833333333333333333333) / x;
            }
            else
            {
                q += Polevl(p, A, 4) / x;
            }

            return q;
        }

        /// <summary>
        /// Returns the natural logarithm of gamma function.
        /// </summary>
        public static double Lgam(double x)
        {
            double p, q, w, z;

            double[] A = {
                         8.11614167470508450300E-4,
                         -5.95061904284301438324E-4,
                         7.93650340457716943945E-4,
                         -2.77777777730099687205E-3,
                         8.33333333333331927722E-2
                     };
            double[] B = {
                         -1.37825152569120859100E3,
                         -3.88016315134637840924E4,
                         -3.31612992738871184744E5,
                         -1.16237097492762307383E6,
                         -1.72173700820839662146E6,
                         -8.53555664245765465627E5
                     };
            double[] C = {
						 /* 1.00000000000000000000E0, */
						 -3.51815701436523470549E2,
                         -1.70642106651881159223E4,
                         -2.20528590553854454839E5,
                         -1.13933444367982507207E6,
                         -2.53252307177582951285E6,
                         -2.01889141433532773231E6
                     };

            if (x < -34.0)
            {
                q = -x;
                w = Lgam(q);
                p = Math.Floor(q);
                if (p == q)
                {
                    throw new ArithmeticException("Lgam: Overflow");
                }

                z = q - p;
                if (z > 0.5)
                {
                    p += 1.0;
                    z = p - q;
                }
                z = q * Math.Sin(Math.PI * z);
                if (z == 0.0)
                {
                    throw new
                                  ArithmeticException("Lgam: Overflow");
                }

                z = LOGPI - Math.Log(z) - w;
                return z;
            }

            if (x < 13.0)
            {
                z = 1.0;
                while (x >= 3.0)
                {
                    x -= 1.0;
                    z *= x;
                }
                while (x < 2.0)
                {
                    if (x == 0.0)
                    {
                        throw new
                                      ArithmeticException("Lgam: Overflow");
                    }

                    z /= x;
                    x += 1.0;
                }
                if (z < 0.0)
                {
                    z = -z;
                }

                if (x == 2.0)
                {
                    return Math.Log(z);
                }

                x -= 2.0;
                p = x * Polevl(x, B, 5) / P1evl(x, C, 6);
                return (Math.Log(z) + p);
            }

            if (x > 2.556348e305)
            {
                throw new ArithmeticException("Lgam: Overflow");
            }

            q = (x - 0.5) * Math.Log(x) - x + 0.91893853320467274178;
            if (x > 1.0e8)
            {
                return (q);
            }

            p = 1.0 / (x * x);
            if (x >= 1000.0)
            {
                q += ((7.9365079365079365079365e-4 * p
                    - 2.7777777777777777777778e-3) * p
                    + 0.0833333333333333333333) / x;
            }
            else
            {
                q += Polevl(p, A, 4) / x;
            }

            return q;
        }

        /// <summary>
        /// Polynomial of degree N
        /// </summary>
        static double Polevl(double x, double[] coef, int N)
        {
            double ans;

            ans = coef[0];

            for (int i = 1; i <= N; i++)
            {
                ans = ans * x + coef[i];
            }

            return ans;
        }

        /// <summary>
        /// Polynomial of degree N with assumtion that coef[N] = 1.0
        /// </summary>
        static double P1evl(double x, double[] coef, int N)
        {
            double ans;

            ans = x + coef[0];

            for (int i = 1; i < N; i++)
            {
                ans = ans * x + coef[i];
            }

            return ans;
        }

        internal static void DefineMatrix(BitArray data, int M, int Q, ref bool[,] m, int k)
        {
            int i, j;

            for (i = 0; i < M; i++)
            {
                for (j = 0; j < Q; j++)
                {
                    m[i, j] = data[k * (M * Q) + j + i * M];
                }
            }
        }

        /// <summary>
        /// Rank of the matrix
        /// </summary>
        internal static int ComputeRank(int M, int Q, bool[,] matrix)
        {
            int i, rank, m = Math.Min(M, Q);

            /* FORWARD APPLICATION OF ELEMENTARY ROW OPERATIONS */
            for (i = 0; i < m - 1; i++)
            {
                if (matrix[i, i])
                {
                    PerformElementaryRowOperations(MatrixOpertion.ForwardElimination, i, M, Q, matrix);
                }
                else
                {   /* matrix[i][i] = 0 */
                    if (FindUnitElementAndSwap(MatrixOpertion.ForwardElimination, i, M, Q, matrix))
                    {
                        PerformElementaryRowOperations(MatrixOpertion.ForwardElimination, i, M, Q, matrix);
                    }
                }
            }

            /* BACKWARD APPLICATION OF ELEMENTARY ROW OPERATIONS */
            for (i = m - 1; i > 0; i--)
            {
                if (matrix[i, i])
                {
                    PerformElementaryRowOperations(MatrixOpertion.BackwardElimination, i, M, Q, matrix);
                }
                else
                {   /* matrix[i][i] = 0 */
                    if (FindUnitElementAndSwap(MatrixOpertion.BackwardElimination, i, M, Q, matrix))
                    {
                        PerformElementaryRowOperations(MatrixOpertion.BackwardElimination, i, M, Q, matrix);
                    }
                }
            }

            rank = DetermineRank(m, M, Q, matrix);

            return rank;
        }

        enum MatrixOpertion { ForwardElimination, BackwardElimination }
        static void PerformElementaryRowOperations(MatrixOpertion flag, int i, int M, int Q, bool[,] A)
        {
            int j, k;

            if (flag == MatrixOpertion.ForwardElimination)
            {
                for (j = i + 1; j < M; j++)
                {
                    if (A[j, i])
                    {
                        for (k = i; k < Q; k++)
                        {
                            A[j, k] = A[j, k] ^ A[i, k];
                        }
                    }
                }
            }
            else
            {
                for (j = i - 1; j >= 0; j--)
                {
                    if (A[j, i])
                    {
                        for (k = 0; k < Q; k++)
                        {
                            A[j, k] = A[j, k] ^ A[i, k];
                        }
                    }
                }
            }
        }

        static bool FindUnitElementAndSwap(MatrixOpertion flag, int i, int M, int Q, bool[,] A)
        {
            int index;

            if (flag == MatrixOpertion.ForwardElimination)
            {
                index = i + 1;
                while ((index < M) && (!A[index, i]))
                {
                    index++;
                }

                if (index < M)
                {
                    SwapRows(i, index, Q, A);
                    return true;
                }
            }
            else
            {
                index = i - 1;
                while ((index >= 0) && (!A[index, i]))
                {
                    index--;
                }

                if (index >= 0)
                {
                    SwapRows(i, index, Q, A);
                    return true;
                }
            }

            return false;
        }

        static int SwapRows(int i, int index, int Q, bool[,] A)
        {
            int p;
            bool temp;

            for (p = 0; p < Q; p++)
            {
                temp = A[i, p];
                A[i, p] = A[index, p];
                A[index, p] = temp;
            }

            return 1;
        }

        static int DetermineRank(int m, int M, int Q, bool[,] A)
        {
            int i, j, rank, allZeroes;

            /* DETERMINE RANK, THAT IS, COUNT THE NUMBER OF NONZERO ROWS */

            rank = m;
            for (i = 0; i < M; i++)
            {
                allZeroes = 1;
                for (j = 0; j < Q; j++)
                {
                    if (A[i, j])
                    {
                        allZeroes = 0;
                        break;
                    }
                }
                if (allZeroes == 1)
                {
                    rank--;
                }
            }

            return rank;
        }

        /// <summary>
        /// Returns the area under the Gaussian probability density function, integrated from minus infinity to a
        /// </summary>
        public static double Normal(double a)
        {
            double x, y, z;

            x = a * SQRTH;
            z = Math.Abs(x);

            if (z < SQRTH)
            {
                y = 0.5 + 0.5 * Erf(x);
            }
            else
            {
                y = 0.5 * Erfc(z);
                if (x > 0)
                {
                    y = 1.0 - y;
                }
            }

            return y;
        }

        /// <summary>
        /// Returns the error function of the specified number
        /// </summary>
        public static double Erf(double x)
        {
            double y, z;
            double[] T = {
                         9.60497373987051638749E0,
                         9.00260197203842689217E1,
                         2.23200534594684319226E3,
                         7.00332514112805075473E3,
                         5.55923013010394962768E4
                     };
            double[] U = {
						 //1.00000000000000000000E0,
						 3.35617141647503099647E1,
                         5.21357949780152679795E2,
                         4.59432382970980127987E3,
                         2.26290000613890934246E4,
                         4.92673942608635921086E4
                     };

            if (Math.Abs(x) > 1.0)
            {
                return (1.0 - Erfc(x));
            }

            z = x * x;
            y = x * Polevl(z, T, 4) / P1evl(z, U, 5);
            return y;
        }
    }
}
