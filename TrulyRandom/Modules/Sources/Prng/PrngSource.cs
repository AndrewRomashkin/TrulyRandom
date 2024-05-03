using System;
using System.Threading;
using TrulyRandom.Models;

namespace TrulyRandom.Modules.Sources.Prng;

/// <summary>
/// Generates data using pseudorandom number generator. This source is unsecure and should never be used for cryptographical purposes, only for testing or debugging!
/// </summary>
[Obsolete("This source is unsecure and should be used only for testing or debugging purposes")]
public abstract class PrngSource : Source
{
    //Is algorithm already initialized.
    private bool initialized;

    //Algorithm seed.
    private byte[] seed;

    /// <summary>
    /// Seed size (in bytes).
    /// </summary>
    public abstract int SeedSize { get; }

    /// <summary>
    /// Gets new data from the algorithm.
    /// </summary>
    protected abstract byte[] NextBytes();

    /// <summary>
    /// Initializes the algorithm.
    /// </summary>
    protected abstract void Initialize(byte[] seed);

    /// <summary>
    /// Gets a requested amount of data from the algorithm.
    /// </summary>
    private byte[] GetBytes(int count)
    {
        if (!initialized)
        {
            if (seed == null)
            {
                seed = Utils.GetSystemRandom(SeedSize);
            }
            Initialize(PadOrCrop(seed, SeedSize));
            initialized = true;
        }
        byte[] result = new byte[count];
        int n = 0;
        while (true)
        {
            byte[] data = NextBytes();
            for (int i = 0; i < data.Length; i++)
            {
                result[n++] = data[i];
                if (n == count)
                {
                    return result;
                }
            }
        }
    }

    /// <summary>
    /// Ensures data has a specified length.
    /// </summary>
    private static byte[] PadOrCrop(byte[] data, int length)
    {
        if (data.Length < length)
        {
            byte[] result = new byte[length];
            Array.Copy(data, 0, result, 0, data.Length);
            byte[] padding = Utils.GetSystemRandom(length - data.Length);
            Array.Copy(padding, 0, result, data.Length, padding.Length);
            return result;
        }
        if (data.Length > length)
        {
            byte[] result = new byte[length];
            Array.Copy(data, 0, result, 0, length);
            return result;
        }
        return data;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrngSource"/> class.
    /// </summary>
    protected PrngSource()
    {
        thread = new Thread(new ThreadStart(WorkerThread));
        thread.IsBackground = true;
        thread.Name = Name;
    }

    /// <summary>
    /// Sets the algorithm seed.
    /// </summary>
    /// <param name="seed">Algorithm seed.</param>
    public void Seed(byte[] seed)
    {
        this.seed = seed;
    }

    /// <summary>
    /// Main loop.
    /// </summary>
    private void WorkerThread()
    {
        while (!shouldBeDisposed)
        {
            if (!Run)
            {
                Thread.Sleep(10);
                continue;
            }
            AddData(GetBytes(BufferSize - BytesInBuffer));
        }
    }
}
