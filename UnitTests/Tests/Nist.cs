using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using TrulyRandom;

namespace UnitTests
{
    /// <summary>
    /// Evaluates the functionality of NIST SP 800-22 test implementation
    /// </summary>
    [TestClass]
    public class Nist
    {
        [TestClass]
        public class ExamplesFromPaper
        {
            [TestMethod]
            public void Frequency()
            {
                NistTests.SingleTestResult result = NistTests.Frequency("1100100100001111110110101010001000100001011010001100001000110100110001001100011001100010100010111000".ToBitArray());
                Assert.AreEqual(0.110, Math.Round(result.PValues[0], 3));
            }

            [TestMethod]
            public void BlockFrequency()
            {
                NistTests.SingleTestResult result = NistTests.BlockFrequency("1100100100001111110110101010001000100001011010001100001000110100110001001100011001100010100010111000".ToBitArray(), 10);
                Assert.AreEqual(0.706, Math.Round(result.PValues[0], 3));
            }

            [TestMethod]
            public void Runs()
            {
                NistTests.SingleTestResult result = NistTests.Runs("1100100100001111110110101010001000100001011010001100001000110100110001001100011001100010100010111000".ToBitArray());
                Assert.AreEqual(0.501, Math.Round(result.PValues[0], 3));
            }

            [TestMethod]
            public void LongestRunOfOnes()
            {
                NistTests.SingleTestResult result = NistTests.LongestRunOfOnes("11001100000101010110110001001100111000000000001001001101010100010001001111010110100000001101011111001100111001101101100010110010".ToBitArray());
                Assert.AreEqual(0.181, Math.Round(result.PValues[0], 3));
            }

            [TestMethod]
            public void BinaryMatrixRank()
            {
                NistTests.SingleTestResult result = NistTests.BinaryMatrixRank("01011001001010101101".ToBitArray(), 3);
                Assert.AreEqual(0.821, Math.Round(result.PValues[0], 3));
            }

            [TestMethod]
            public void DiscreteFourierTransform()
            {
                NistTests.SingleTestResult result = NistTests.DiscreteFourierTransform("1001010011".ToBitArray());
                Assert.AreEqual(0.468, Math.Round(result.PValues[0], 3));
            }

            [TestMethod]
            public void NonOverlappingTemplateMatching()
            {
                NistTests.SingleTestResult result = NistTests.NonOverlappingTemplateMatchings("10100100101110010110".ToBitArray(), "001".ToBitArray(), 10);
                Assert.AreEqual(0.344, Math.Round(result.PValues[0], 3));
            }

            [TestMethod]
            public void OverlappingTemplateMatching()
            {
                NistTests.SingleTestResult result = NistTests.OverlappingTemplateMatchings("10111011110010110100011100101110111110000101101001".ToBitArray(), "11".ToBitArray(), 5, 10);
                Assert.AreEqual(0.410, Math.Round(result.PValues[0], 3));
            }

            [TestMethod]
            public void MaurersUniversal()
            {
                NistTests.SingleTestResult result = NistTests.MaurersUniversal("01011010011101010111".ToBitArray(), 2, 4);
                Assert.AreEqual(0.063, Math.Round(result.PValues[0], 3));
            }

            [TestMethod]
            public void LinearComplexity()
            {
                NistTests.SingleTestResult result = NistTests.LinearComplexity(Utils.First1mDigitsOfE.ToBitArray(), 1000, 3);
                Assert.AreEqual(0.524, Math.Round(result.PValues[0], 3));
            }

            [TestMethod]
            public void Serial()
            {
                NistTests.SingleTestResult result = NistTests.Serial("0011011101".ToBitArray(), 3);
                Assert.AreEqual(0.809, Math.Round(result.PValues[0], 3));
                Assert.AreEqual(0.670, Math.Round(result.PValues[1], 3));
            }

            [TestMethod]
            public void ApproximateEntropy()
            {
                NistTests.SingleTestResult result = NistTests.ApproximateEntropy("0100110101".ToBitArray(), 3);
                Assert.AreEqual(0.262, Math.Round(result.PValues[0], 3));
            }

            [TestMethod]
            public void CumulativeSums()
            {
                NistTests.SingleTestResult result = NistTests.CumulativeSums("1011010111".ToBitArray());
                Assert.AreEqual(0.412, Math.Round(result.PValues[0], 3));
                Assert.AreEqual(0.412, Math.Round(result.PValues[1], 3));
            }

            [TestMethod]    
            public void RandomExcursions()
            {
                NistTests.SingleTestResult result = NistTests.RandomExcursions(Utils.First1mDigitsOfE.ToBitArray());
                Assert.AreEqual(0.571, Math.Round(result.PValues[0], 3));
                Assert.AreEqual(0.197, Math.Round(result.PValues[1], 3));
                Assert.AreEqual(0.166, Math.Round(result.PValues[2], 3));
                Assert.AreEqual(0.008, Math.Round(result.PValues[3], 3));
                Assert.AreEqual(0.786, Math.Round(result.PValues[4], 3));
                Assert.AreEqual(0.444, Math.Round(result.PValues[5], 3));
                Assert.AreEqual(0.799, Math.Round(result.PValues[6], 3));
                Assert.AreEqual(0.778, Math.Round(result.PValues[7], 3));
            }

            [TestMethod]
            public void RandomExcursionsVariant()
            {
                NistTests.SingleTestResult result = NistTests.RandomExcursionsVariant(Utils.First1mDigitsOfE.ToBitArray());
                Assert.AreEqual(0.862, Math.Round(result.PValues[0], 3));
                Assert.AreEqual(0.798, Math.Round(result.PValues[1], 3));
                Assert.AreEqual(0.580, Math.Round(result.PValues[2], 3));
                Assert.AreEqual(0.497, Math.Round(result.PValues[3], 3));
                Assert.AreEqual(0.638, Math.Round(result.PValues[4], 3));
                Assert.AreEqual(0.923, Math.Round(result.PValues[5], 3));
                Assert.AreEqual(0.941, Math.Round(result.PValues[6], 3));
                Assert.AreEqual(0.824, Math.Round(result.PValues[7], 3));
                Assert.AreEqual(0.812, Math.Round(result.PValues[8], 3));
                Assert.AreEqual(0.138, Math.Round(result.PValues[9], 3));
                Assert.AreEqual(0.204, Math.Round(result.PValues[10], 3));
                Assert.AreEqual(0.446, Math.Round(result.PValues[11], 3));
                Assert.AreEqual(0.945, Math.Round(result.PValues[12], 3));
                Assert.AreEqual(0.502, Math.Round(result.PValues[13], 3));
                Assert.AreEqual(0.442, Math.Round(result.PValues[14], 3));
                Assert.AreEqual(0.509, Math.Round(result.PValues[15], 3));
                Assert.AreEqual(0.535, Math.Round(result.PValues[16], 3));
                Assert.AreEqual(0.591, Math.Round(result.PValues[17], 3));
            }
        }

        [TestClass]
        public class OneMBits
        {
            [TestMethod]
            public void Frequency()
            {
                NistTests.SingleTestResult result = NistTests.Frequency(Utils.First1mDigitsOfE.ToBitArray());
                Assert.AreEqual(0.952, Math.Round(result.PValues[0], 3));
            }

            [TestMethod]
            public void BlockFrequency()
            {
                NistTests.SingleTestResult result = NistTests.BlockFrequency(Utils.First1mDigitsOfE.ToBitArray(), 200000);
                Assert.AreEqual(0.235, Math.Round(result.PValues[0], 3));
            }

            [TestMethod]
            public void Runs()
            {
                NistTests.SingleTestResult result = NistTests.Runs(Utils.First1mDigitsOfE.ToBitArray());
                Assert.AreEqual(0.561, Math.Round(result.PValues[0], 3));
            }

            [TestMethod]
            public void LongestRunOfOnes()
            {
                NistTests.SingleTestResult result = NistTests.LongestRunOfOnes(Utils.First1mDigitsOfE.ToBitArray());
                Assert.AreEqual(0.719, Math.Round(result.PValues[0], 3));
            }

            [TestMethod]
            public void BinaryMatrixRank()
            {
                NistTests.SingleTestResult result = NistTests.BinaryMatrixRank(Utils.First1mDigitsOfE.ToBitArray(), 50);
                Assert.AreEqual(0.171, Math.Round(result.PValues[0], 3));
            }

            [TestMethod]
            public void DiscreteFourierTransform()
            {
                NistTests.SingleTestResult result = NistTests.DiscreteFourierTransform(Utils.First1mDigitsOfE.ToBitArray());
                Assert.AreEqual(0.804, Math.Round(result.PValues[0], 3));
            }

            [TestMethod]
            public void NonOverlappingTemplateMatching()
            {
                NistTests.SingleTestResult result = NistTests.NonOverlappingTemplateMatchings(Utils.First1mDigitsOfE.ToBitArray(), 9, 10000);
                Assert.IsTrue(result.PValues.Where(x => x < 0.01).Count() == 1);
            }

            [TestMethod]
            public void OverlappingTemplateMatching()
            {
                NistTests.SingleTestResult result = NistTests.OverlappingTemplateMatchings(Utils.First1mDigitsOfE.ToBitArray(), 9, 5, 1032);
                Assert.AreEqual(0.096, Math.Round(result.PValues[0], 3));
            }

            [TestMethod]
            public void MaurersUniversal()
            {
                NistTests.SingleTestResult result = NistTests.MaurersUniversal(Utils.First1mDigitsOfE.ToBitArray(), 7, 1280);
                Assert.AreEqual(0.445, Math.Round(result.PValues[0], 3));
            }

            [TestMethod]
            public void LinearComplexity()
            {
                NistTests.SingleTestResult result = NistTests.LinearComplexity(Utils.First1mDigitsOfE.ToBitArray(), 500, 3);
                Assert.AreEqual(0.629, Math.Round(result.PValues[0], 3));
            }

            [TestMethod]
            public void Serial()
            {
                NistTests.SingleTestResult result = NistTests.Serial(Utils.First1mDigitsOfE.ToBitArray(), 2);
                Assert.AreEqual(0.842, Math.Round(result.PValues[0], 3));
                Assert.AreEqual(0.559, Math.Round(result.PValues[1], 3));
            }

            [TestMethod]
            public void ApproximateEntropy()
            {
                NistTests.SingleTestResult result = NistTests.ApproximateEntropy(Utils.First1mDigitsOfE.ToBitArray(), 2);
                Assert.AreEqual(0.695, Math.Round(result.PValues[0], 3));
            }

            [TestMethod]
            public void CumulativeSums()
            {
                NistTests.SingleTestResult result = NistTests.CumulativeSums(Utils.First1mDigitsOfE.ToBitArray());
                Assert.AreEqual(0.67, Math.Round(result.PValues[0], 3));
            }

            [TestMethod]
            public void RandomExcursions()
            {
                NistTests.SingleTestResult result = NistTests.RandomExcursions(Utils.First1mDigitsOfE.ToBitArray());
                Assert.AreEqual(0.571, Math.Round(result.PValues[0], 3));
                Assert.AreEqual(0.197, Math.Round(result.PValues[1], 3));
                Assert.AreEqual(0.166, Math.Round(result.PValues[2], 3));
                Assert.AreEqual(0.008, Math.Round(result.PValues[3], 3));
                Assert.AreEqual(0.786, Math.Round(result.PValues[4], 3));
                Assert.AreEqual(0.444, Math.Round(result.PValues[5], 3));
                Assert.AreEqual(0.799, Math.Round(result.PValues[6], 3));
                Assert.AreEqual(0.778, Math.Round(result.PValues[7], 3));
            }

            [TestMethod]
            public void RandomExcursionsVariant()
            {
                NistTests.SingleTestResult result = NistTests.RandomExcursionsVariant(Utils.First1mDigitsOfE.ToBitArray());
                Assert.AreEqual(0.862, Math.Round(result.PValues[0], 3));
                Assert.AreEqual(0.798, Math.Round(result.PValues[1], 3));
                Assert.AreEqual(0.580, Math.Round(result.PValues[2], 3));
                Assert.AreEqual(0.497, Math.Round(result.PValues[3], 3));
                Assert.AreEqual(0.638, Math.Round(result.PValues[4], 3));
                Assert.AreEqual(0.923, Math.Round(result.PValues[5], 3));
                Assert.AreEqual(0.941, Math.Round(result.PValues[6], 3));
                Assert.AreEqual(0.824, Math.Round(result.PValues[7], 3));
                Assert.AreEqual(0.812, Math.Round(result.PValues[8], 3));
                Assert.AreEqual(0.138, Math.Round(result.PValues[9], 3));
                Assert.AreEqual(0.204, Math.Round(result.PValues[10], 3));
                Assert.AreEqual(0.446, Math.Round(result.PValues[11], 3));
                Assert.AreEqual(0.945, Math.Round(result.PValues[12], 3));
                Assert.AreEqual(0.502, Math.Round(result.PValues[13], 3));
                Assert.AreEqual(0.442, Math.Round(result.PValues[14], 3));
                Assert.AreEqual(0.509, Math.Round(result.PValues[15], 3));
                Assert.AreEqual(0.535, Math.Round(result.PValues[16], 3));
                Assert.AreEqual(0.591, Math.Round(result.PValues[17], 3));
            }
        }
    }
}
