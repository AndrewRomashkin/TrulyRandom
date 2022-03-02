using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using TrulyRandom;

namespace UnitTests
{
    [TestClass]
    public class Nist
    {
        [TestClass]
        public class ExamplesFromPaper
        {
            [TestMethod]
            public void Frequency()
            {
                NistTests.RawTestResult result = NistTests.Frequency("1100100100001111110110101010001000100001011010001100001000110100110001001100011001100010100010111000".ToBitArray());
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.110);
            }

            [TestMethod]
            public void BlockFrequency()
            {
                NistTests.RawTestResult result = NistTests.BlockFrequency("1100100100001111110110101010001000100001011010001100001000110100110001001100011001100010100010111000".ToBitArray(), 10);
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.706);
            }

            [TestMethod]
            public void Runs()
            {
                NistTests.RawTestResult result = NistTests.Runs("1100100100001111110110101010001000100001011010001100001000110100110001001100011001100010100010111000".ToBitArray());
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.501);
            }

            [TestMethod]
            public void LongestRunOfOnes()
            {
                NistTests.RawTestResult result = NistTests.LongestRunOfOnes("11001100000101010110110001001100111000000000001001001101010100010001001111010110100000001101011111001100111001101101100010110010".ToBitArray());
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.181);
            }

            [TestMethod]
            public void BinaryMatrixRank()
            {
                NistTests.RawTestResult result = NistTests.BinaryMatrixRank("01011001001010101101".ToBitArray(), 3);
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.821);
            }

            [TestMethod]
            public void DiscreteFourierTransform()
            {
                NistTests.RawTestResult result = NistTests.DiscreteFourierTransform("1001010011".ToBitArray());
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.468);
            }

            [TestMethod]
            public void NonOverlappingTemplateMatching()
            {
                NistTests.RawTestResult result = NistTests.NonOverlappingTemplateMatchings("10100100101110010110".ToBitArray(), "001".ToBitArray(), 10);
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.344);
            }

            [TestMethod]
            public void OverlappingTemplateMatching()
            {
                NistTests.RawTestResult result = NistTests.OverlappingTemplateMatchings("10111011110010110100011100101110111110000101101001".ToBitArray(), "11".ToBitArray(), 5, 10);
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.410);
            }

            [TestMethod]
            public void MaurersUniversal()
            {
                NistTests.RawTestResult result = NistTests.MaurersUniversal("01011010011101010111".ToBitArray(), 2, 4);
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.063);
            }

            [TestMethod]
            public void LinearComplexity()
            {
                NistTests.RawTestResult result = NistTests.LinearComplexity(Utils.First1mDigitsOfE.ToBitArray(), 1000, 3);
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.524);
            }

            [TestMethod]
            public void Serial()
            {
                NistTests.RawTestResult result = NistTests.Serial("0011011101".ToBitArray(), 3);
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.809);
                Assert.IsTrue(Math.Round(result.PValues[1], 3) == 0.670);
            }

            [TestMethod]
            public void ApproximateEntropy()
            {
                NistTests.RawTestResult result = NistTests.ApproximateEntropy("0100110101".ToBitArray(), 3);
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.262);
            }

            [TestMethod]
            public void CumulativeSums()
            {
                NistTests.RawTestResult result = NistTests.CumulativeSums("1011010111".ToBitArray());
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.412);
                Assert.IsTrue(Math.Round(result.PValues[1], 3) == 0.412);
            }

            [TestMethod]
            public void RandomExcursions()
            {
                NistTests.RawTestResult result = NistTests.RandomExcursions(Utils.First1mDigitsOfE.ToBitArray());
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.571);
                Assert.IsTrue(Math.Round(result.PValues[1], 3) == 0.197);
                Assert.IsTrue(Math.Round(result.PValues[2], 3) == 0.166);
                Assert.IsTrue(Math.Round(result.PValues[3], 3) == 0.008);
                Assert.IsTrue(Math.Round(result.PValues[4], 3) == 0.786);
                Assert.IsTrue(Math.Round(result.PValues[5], 3) == 0.444);
                Assert.IsTrue(Math.Round(result.PValues[6], 3) == 0.799);
                Assert.IsTrue(Math.Round(result.PValues[7], 3) == 0.778);
            }

            [TestMethod]
            public void RandomExcursionsVariant()
            {
                NistTests.RawTestResult result = NistTests.RandomExcursionsVariant(Utils.First1mDigitsOfE.ToBitArray());
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.862);
                Assert.IsTrue(Math.Round(result.PValues[1], 3) == 0.798);
                Assert.IsTrue(Math.Round(result.PValues[2], 3) == 0.580);
                Assert.IsTrue(Math.Round(result.PValues[3], 3) == 0.497);
                Assert.IsTrue(Math.Round(result.PValues[4], 3) == 0.638);
                Assert.IsTrue(Math.Round(result.PValues[5], 3) == 0.923);
                Assert.IsTrue(Math.Round(result.PValues[6], 3) == 0.941);
                Assert.IsTrue(Math.Round(result.PValues[7], 3) == 0.824);
                Assert.IsTrue(Math.Round(result.PValues[8], 3) == 0.812);
                Assert.IsTrue(Math.Round(result.PValues[9], 3) == 0.138);
                Assert.IsTrue(Math.Round(result.PValues[10], 3) == 0.204);
                Assert.IsTrue(Math.Round(result.PValues[11], 3) == 0.446);
                Assert.IsTrue(Math.Round(result.PValues[12], 3) == 0.945);
                Assert.IsTrue(Math.Round(result.PValues[13], 3) == 0.502);
                Assert.IsTrue(Math.Round(result.PValues[14], 3) == 0.442);
                Assert.IsTrue(Math.Round(result.PValues[15], 3) == 0.509);
                Assert.IsTrue(Math.Round(result.PValues[16], 3) == 0.535);
                Assert.IsTrue(Math.Round(result.PValues[17], 3) == 0.591);
            }
        }

        [TestClass]
        public class OneMBits
        {
            [TestMethod]
            public void Frequency()
            {
                NistTests.RawTestResult result = NistTests.Frequency(Utils.First1mDigitsOfE.ToBitArray());
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.952);
            }

            [TestMethod]
            public void BlockFrequency()
            {
                NistTests.RawTestResult result = NistTests.BlockFrequency(Utils.First1mDigitsOfE.ToBitArray(), 200000);
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.235);
            }

            [TestMethod]
            public void Runs()
            {
                NistTests.RawTestResult result = NistTests.Runs(Utils.First1mDigitsOfE.ToBitArray());
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.561);
            }

            [TestMethod]
            public void LongestRunOfOnes()
            {
                NistTests.RawTestResult result = NistTests.LongestRunOfOnes(Utils.First1mDigitsOfE.ToBitArray());
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.719);
            }

            [TestMethod]
            public void BinaryMatrixRank()
            {
                NistTests.RawTestResult result = NistTests.BinaryMatrixRank(Utils.First1mDigitsOfE.ToBitArray(), 50);
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.171);
            }

            [TestMethod]
            public void DiscreteFourierTransform()
            {
                NistTests.RawTestResult result = NistTests.DiscreteFourierTransform(Utils.First1mDigitsOfE.ToBitArray());
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.804);
            }

            [TestMethod]
            public void NonOverlappingTemplateMatching()
            {
                NistTests.RawTestResult result = NistTests.NonOverlappingTemplateMatchings(Utils.First1mDigitsOfE.ToBitArray(), 9, 10000);
                Assert.IsTrue(result.PValues.Where(x => x < 0.01).Count() == 1);
            }

            [TestMethod]
            public void OverlappingTemplateMatching()
            {
                NistTests.RawTestResult result = NistTests.OverlappingTemplateMatchings(Utils.First1mDigitsOfE.ToBitArray(), 9, 5, 1032);
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.096);
            }

            [TestMethod]
            public void MaurersUniversal()
            {
                NistTests.RawTestResult result = NistTests.MaurersUniversal(Utils.First1mDigitsOfE.ToBitArray(), 7, 1280);
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.445);
            }

            [TestMethod]
            public void LinearComplexity()
            {
                NistTests.RawTestResult result = NistTests.LinearComplexity(Utils.First1mDigitsOfE.ToBitArray(), 500, 3);
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.629);
            }

            [TestMethod]
            public void Serial()
            {
                NistTests.RawTestResult result = NistTests.Serial(Utils.First1mDigitsOfE.ToBitArray(), 2);
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.842);
                Assert.IsTrue(Math.Round(result.PValues[1], 3) == 0.559);
            }

            [TestMethod]
            public void ApproximateEntropy()
            {
                NistTests.RawTestResult result = NistTests.ApproximateEntropy(Utils.First1mDigitsOfE.ToBitArray(), 2);
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.695);
            }

            [TestMethod]
            public void CumulativeSums()
            {
                NistTests.RawTestResult result = NistTests.CumulativeSums(Utils.First1mDigitsOfE.ToBitArray());
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.67);
            }

            [TestMethod]
            public void RandomExcursions()
            {
                NistTests.RawTestResult result = NistTests.RandomExcursions(Utils.First1mDigitsOfE.ToBitArray());
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.571);
                Assert.IsTrue(Math.Round(result.PValues[1], 3) == 0.197);
                Assert.IsTrue(Math.Round(result.PValues[2], 3) == 0.166);
                Assert.IsTrue(Math.Round(result.PValues[3], 3) == 0.008);
                Assert.IsTrue(Math.Round(result.PValues[4], 3) == 0.786);
                Assert.IsTrue(Math.Round(result.PValues[5], 3) == 0.444);
                Assert.IsTrue(Math.Round(result.PValues[6], 3) == 0.799);
                Assert.IsTrue(Math.Round(result.PValues[7], 3) == 0.778);
            }

            [TestMethod]
            public void RandomExcursionsVariant()
            {
                NistTests.RawTestResult result = NistTests.RandomExcursionsVariant(Utils.First1mDigitsOfE.ToBitArray());
                Assert.IsTrue(Math.Round(result.PValues[0], 3) == 0.862);
                Assert.IsTrue(Math.Round(result.PValues[1], 3) == 0.798);
                Assert.IsTrue(Math.Round(result.PValues[2], 3) == 0.580);
                Assert.IsTrue(Math.Round(result.PValues[3], 3) == 0.497);
                Assert.IsTrue(Math.Round(result.PValues[4], 3) == 0.638);
                Assert.IsTrue(Math.Round(result.PValues[5], 3) == 0.923);
                Assert.IsTrue(Math.Round(result.PValues[6], 3) == 0.941);
                Assert.IsTrue(Math.Round(result.PValues[7], 3) == 0.824);
                Assert.IsTrue(Math.Round(result.PValues[8], 3) == 0.812);
                Assert.IsTrue(Math.Round(result.PValues[9], 3) == 0.138);
                Assert.IsTrue(Math.Round(result.PValues[10], 3) == 0.204);
                Assert.IsTrue(Math.Round(result.PValues[11], 3) == 0.446);
                Assert.IsTrue(Math.Round(result.PValues[12], 3) == 0.945);
                Assert.IsTrue(Math.Round(result.PValues[13], 3) == 0.502);
                Assert.IsTrue(Math.Round(result.PValues[14], 3) == 0.442);
                Assert.IsTrue(Math.Round(result.PValues[15], 3) == 0.509);
                Assert.IsTrue(Math.Round(result.PValues[16], 3) == 0.535);
                Assert.IsTrue(Math.Round(result.PValues[17], 3) == 0.591);
            }
        }
    }
}
