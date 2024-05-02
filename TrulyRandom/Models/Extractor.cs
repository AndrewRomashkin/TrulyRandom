using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace TrulyRandom.Models
{
    /// <summary>
    /// Base class for all extractors - modules that read data from other modules (sources or other extractors) and maximizes its entropy, removes dependencies, etc
    /// </summary>
    public abstract class Extractor : Module
    {
        /// <summary>
        /// Shows how much is data compressed by this extractor
        /// </summary>
        public double ActualCompression { get; protected set; } = 0;
        /// <summary>
        /// Sources of data for this extractor
        /// </summary>
        public Module[] Sources { get; set; } = Array.Empty<Module>();
        /// <summary>
        /// Determines whether data block should be taken from all available sources and concatenated (<c>true</c>), or from one source if possible (<c>false</c>)
        /// </summary>
        public bool MixDataFromDifferentSources { get; set; } = true;
        /// <summary>
        /// Shows if extractor is idle due to lack of data in its sources
        /// </summary>
        public bool NoDataToProcess { get; private set; } = false;
        /// <summary>
        /// Total amount of bytes read by this extractor from its sources
        /// </summary>
        public ulong TotalBytesConsumed { get; private set; } = 0;

        /// <summary>
        /// Determines whether child class uses default compression calculator, or its own
        /// </summary>
        protected abstract bool UseDefaultCompressionCalculator { get; }
        /// <summary>
        /// Determines whether child class implements <see cref="ISeedable"/> interface and can be seeded
        /// </summary>
        protected virtual bool Seedable { get; } = false;

        int batchSize = 10_000;
        /// <summary>
        /// Number of bytes read from sources and processed at once
        /// </summary>
        public int BatchSize
        {
            get => batchSize;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("Batch size should be >= 1");
                }
                batchSize = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Extractor" /> class
        /// </summary>
        public Extractor()
        {
            thread = new Thread(WorkerThread);
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.Lowest;
            thread.Name = Name;
        }

        /// <summary>
        /// Processed block history
        /// </summary>
        readonly List<(int Input, int Output, double Time)> history = new();
        readonly Stopwatch stopwatch = new();

        /// <summary>
        /// Main thread
        /// </summary>
        void WorkerThread()
        {
            while (!dispose)
            {
                Module[] sources = Sources;
                if (!Run || sources.Length == 0)
                {
                    if (sources.Length == 0)
                    {
                        NoDataToProcess = true;
                    }
                    Thread.Sleep(10);
                    continue;
                }

                stopwatch.Restart();
                //Reading data
                byte[] inputData = ReadDataFromSources(GetActualBatchSize());
                if (inputData == null || inputData.Length == 0)
                {
                    NoDataToProcess = true;
                    Thread.Sleep(10);
                    continue;
                }
                NoDataToProcess = false;

                //Processing it
                byte[] outputData = ProcessData(inputData);
                //And writing to the buffer
                AddData(outputData);
                stopwatch.Stop();

                TotalBytesConsumed += (uint)inputData.Length;
                //If extractor is seedable - check if we should refresh the seed
                if (Seedable)
                {
                    bytesWithCurrentSeed += (ulong)inputData.Length;
                    RotateSeed(false);
                }

                history.Add((inputData.Length, outputData.Length, stopwatch.Elapsed.TotalSeconds == 0 ? 1 : stopwatch.Elapsed.TotalSeconds));

                int totalInput = history.Sum(x => x.Input);
                //Calculating compression
                if (UseDefaultCompressionCalculator)
                {
                    int totalOutput = history.Sum(x => x.Output);
                    ActualCompression = (double)totalInput / totalOutput;
                }

                //Calculating bytes per second
                if (CalculateBPS)
                {
                    double totalTime = history.Sum(x => x.Time);
                    BytesPerSecond = (int)(totalInput / totalTime);
                }

                while (history.Take(history.Count - 1).Sum(x => x.Output) > Constants.CompressionCalculationBytes)
                {
                    history.RemoveAt(0);
                }
            }
            Disposed = true;
        }

        ulong bytesWithCurrentSeed = 0;

        /// <summary>
        /// Source of seed data. It is recommended to use high-quality entropy from the end of a chain
        /// </summary>
        protected Module seedSource = null;
        /// <summary>
        /// Seed data
        /// </summary>
        protected byte[] seed = null;
        /// <summary>
        /// Length of the seed
        /// </summary>
        protected int seedLength = 64;
        /// <summary>
        /// Number of input bytes after which seed will be rotated
        /// </summary>
        protected int seedRotationInterval = 1000000;

        /// <summary>
        /// Checks if seed should be rotated and rotates if so
        /// </summary>
        /// <param name="force">Force rotate the seed</param>
        protected void RotateSeed(bool force)
        {
            if ((bytesWithCurrentSeed < (ulong)seedRotationInterval && !force && seed != null) || seedSource == null)
            {
                return;
            }
            byte[] newSeed = seedSource.ReadExactly(seedLength);
            if (newSeed.Length != 0)
            {
                bytesWithCurrentSeed = 0;
                seed = newSeed;
            }
        }

        /// <summary>
        /// Calculates bytes per second output of this extractor
        /// </summary>
        protected override void BPSCalculation()
        {
        }

        /// <summary>
        /// Method containing logic for data processing algorithm
        /// </summary>
        /// <param name="data">Data to be processed</param>
        /// <returns>Processed data</returns>
        protected abstract byte[] ProcessData(byte[] data);

        /// <summary>
        /// Adds source to read data from
        /// </summary>
        /// <param name="source"></param>
        public void AddSource(Module source)
        {
            if (Sources.Contains(source))
            {
                return;
            }
            Sources = Sources.Append(source);
        }

        /// <summary>
        /// Removes source from the list
        /// </summary>
        /// <param name="source">Source to be removed</param>
        public void RemoveSource(Module source)
        {
            if (Sources.Contains(source))
            {
                return;
            }
            Sources = Sources.Where(x => x != source).ToArray();
        }

        /// <summary>
        /// Determines whether compression should be adjusted depending on how full is the buffer of the <see cref="DynamicCoefficientSource"/> module
        /// </summary>
        public bool DynamicallyAdjustCompression { get; set; } = true;
        /// <summary>
        /// Module which determines <see cref="DynamicCoefficient"/>
        /// </summary>
        public Module DynamicCoefficientSource { get; set; } = null;

        /// <summary>
        /// Current dynamic coefficient
        /// </summary>
        public double DynamicCoefficient
        {
            get
            {
                Module dynamicCoefficientSource = DynamicCoefficientSource;
                if (!DynamicallyAdjustCompression || dynamicCoefficientSource == null)
                {
                    return 0;
                }
                return dynamicCoefficientSource.BufferState;
            }
        }

        int maxBatchSize = 5_000_000;
        /// <summary>
        /// Maximum length of the batch with respect to dynamic coefficient.
        /// </summary>
        public int MaxBatchSize
        {
            get => maxBatchSize;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentException("Max batch size should be >= 1");
                }
                maxBatchSize = value;
            }
        }

        /// <summary>
        /// Determines the current size of the batch
        /// </summary>
        /// <returns></returns>
        protected abstract int GetActualBatchSize();

        /// <summary>
        /// Reads data from sources
        /// </summary>
        /// <param name="count">Number of bytes to read</param>
        /// <returns>Data read</returns>
        byte[] ReadDataFromSources(int count)
        {
            Module[] sources = Sources;
            if (sources.Length == 0)
            {
                return Array.Empty<byte>();
            }

            List<Module> lockedSources = new();
            try
            {
                foreach (Module source in sources)
                {
                    if (Monitor.TryEnter(source.Sync, 100))
                    {
                        lockedSources.Add(source);
                    }
                }

                if (!lockedSources.Any())
                {
                    return Array.Empty<byte>();
                }

                int totalAvailableBytes = lockedSources.Sum(x => x.BytesInBuffer);

                if (totalAvailableBytes < count)
                {
                    return Array.Empty<byte>();
                }

                byte[][] data = new byte[lockedSources.Count][];
                if (MixDataFromDifferentSources)
                {
                    lockedSources = lockedSources.OrderBy(x => x.BytesInBuffer).ToList();
                    for (int i = 0; i < lockedSources.Count; i++)
                    {
                        data[i] = lockedSources[i].ReadUpTo((count - data.Where(x => x != null).Sum(x => x.Length)) / (lockedSources.Count - i));
                    }
                }
                else
                {
                    for (int i = 0; i < lockedSources.Count; i++)
                    {
                        data[i] = lockedSources[i].ReadUpTo(count - data.Where(x => x != null).Sum(x => x.Length));
                    }
                }

                if (data.Sum(x => x.Length) == 0)
                {
                    return Array.Empty<byte>();
                }

                if (MixDataFromDifferentSources)
                {
                    data = data.Where(x => x != null && x.Length != 0).ToArray();
                    return Utils.MixData(data);
                }
                return data.SelectMany(x => x).ToArray();
            }
            catch
            {
                return Array.Empty<byte>();
            }
            finally
            {
                foreach (Module source in lockedSources)
                {
                    Monitor.Exit(source.Sync);
                }
            }
        }
    }
}
