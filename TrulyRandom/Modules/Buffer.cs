using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using TrulyRandom.Models;

namespace TrulyRandom.Modules
{
    /// <summary>
    /// Allows for data storage both in the output buffer and on the disk
    /// </summary>
    public class Buffer : Module
    {
        /// <summary>
        /// Sources of data for this buffer
        /// </summary>
        public Module[] Sources { get; set; } = Array.Empty<Module>();
        /// <summary>
        /// Size of each of the files stored on the disk
        /// </summary>
        public int BufferFileSize { get; set; } = 1_000_000;
        /// <summary>
        /// Maximum number of files to store on the disk (0 to disable disk storage)
        /// </summary>
        public int MaxFilesToStore { get; set; } = 0;
        /// <summary>
        /// If number of bytes in the buffer is lower than this amount - data will be loaded from the disk
        /// </summary>
        public int MinBytesInBuffer { get; set; } = 100_000;
        /// <summary>
        /// If number of bytes in the buffer is higher than this amount - data will be saved to the disk
        /// </summary>
        public int MaxBytesInBuffer { get; set; } = 2_000_000;
        /// <summary>
        /// Directory in which buffer files will be located
        /// </summary>
        public string BufferDirectory { get; set; } = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "RandomnessBuffer";
        /// <summary>
        /// Determines whether data block should be taken from all available sources and concatenated (<c>true</c>), or from one source if possible (<c>false</c>)
        /// </summary>
        public bool MixDataFromDifferentSources { get; set; } = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Buffer" /> class
        /// </summary>
        public Buffer()
        {
            thread = new Thread(WorkerThread);
            thread.Name = Name;
        }

        /// <summary>
        /// Main loop
        /// </summary>
        void WorkerThread()
        {
            while (!dispose)
            {
                if (!Run)
                {
                    Thread.Sleep(10);
                    continue;
                }

                if (!Overflow && Sources.Sum(x => x.BytesInBuffer) > 0)
                {
                    byte[] data = ReadDataFromSources();
                    if (data.Any())
                    {
                        AddData(data);
                    }
                }

                SaveToDisk();
                LoadFromDisk();
                Thread.Sleep(1);
            }
            Disposed = true;
        }

        readonly object directoryLock = new();
        DateTime nextWriteCheck = DateTime.Now;
        /// <summary>
        /// Saves exccess data to the disk
        /// </summary>
        void SaveToDisk()
        {
            if (DateTime.Now < nextWriteCheck || BytesInBuffer < MaxBytesInBuffer)
            {
                return;
            }
            lock (directoryLock)
            {
                try
                {
                    Directory.CreateDirectory(BufferDirectory);
                    while (BytesInBuffer >= MinBytesInBuffer + BufferFileSize)
                    {
                        List<int> files = new();
                        foreach (string file in Directory.GetFiles(BufferDirectory))
                        {
                            if (int.TryParse(Path.GetFileName(file), out int number))
                            {
                                files.Add(number);
                            }
                        }
                        if (files.Count < MaxFilesToStore)
                        {
                            int fileName = 0;
                            while (files.Contains(fileName))
                            {
                                fileName++;
                            }

                            byte[] data = ReadExactly(BufferFileSize);
                            if (data != null && data.Any())
                            {
                                try
                                {
                                    File.WriteAllBytes(BufferDirectory + Path.DirectorySeparatorChar + fileName, data);
                                }
                                catch
                                {
                                    AddData(data);
                                    nextWriteCheck = DateTime.Now + TimeSpan.FromSeconds(10);
                                }
                            }
                        }
                        else
                        {
                            nextWriteCheck = DateTime.Now + TimeSpan.FromSeconds(5);
                            return;
                        }
                    }
                }
                catch
                {
                    nextWriteCheck = DateTime.Now + TimeSpan.FromSeconds(10);
                    return;
                }
            }
        }

        DateTime nextReadCheck = DateTime.MinValue;
        /// <summary>
        /// Loads insufficient data from buffer files
        /// </summary>
        void LoadFromDisk()
        {
            if (DateTime.Now < nextReadCheck || BytesInBuffer >= MinBytesInBuffer)
            {
                return;
            }
            lock (directoryLock)
            {
                if (!Directory.Exists(BufferDirectory))
                {
                    nextReadCheck = DateTime.Now + TimeSpan.FromSeconds(10);
                    return;
                }
                while (BytesInBuffer < MinBytesInBuffer)
                {
                    List<int> files = new();
                    foreach (string file in Directory.GetFiles(BufferDirectory))
                    {
                        if (int.TryParse(Path.GetFileName(file), out int number))
                        {
                            files.Add(number);
                        }
                    }
                    if (!files.Any())
                    {
                        nextReadCheck = DateTime.Now + TimeSpan.FromSeconds(10);
                        return;
                    }
                    int fileName = files.Max();
                    try
                    {
                        byte[] data = File.ReadAllBytes(BufferDirectory + Path.DirectorySeparatorChar + fileName);
                        File.Delete(BufferDirectory + Path.DirectorySeparatorChar + fileName);
                        AddData(data);
                    }
                    catch
                    {
                        nextReadCheck = DateTime.Now + TimeSpan.FromSeconds(10);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Deletes data stored on the disk
        /// </summary>
        public void ClearDiskData()
        {
            lock (directoryLock)
            {
                if (!Directory.Exists(BufferDirectory))
                {
                    return;
                }
                foreach (string file in Directory.GetFiles(BufferDirectory))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// Gets the number of buffer files available on the disk
        /// </summary>
        /// <returns>Number of files available</returns>
        public int GetNumberOfDataFilesAvailable()
        {
            lock (directoryLock)
            {
                if (!Directory.Exists(BufferDirectory))
                {
                    return 0;
                }
                int result = 0;
                foreach (string file in Directory.GetFiles(BufferDirectory))
                {
                    if (int.TryParse(Path.GetFileName(file), out int number))
                    {
                        result++;
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Reads data from sources
        /// </summary>
        /// <returns>Data read</returns>
        byte[] ReadDataFromSources()
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

                int bytesToRead = BufferSize - BytesInBuffer;
                int bytesAvailable = lockedSources.Sum(x => x.BytesInBuffer);
                byte[][] data;

                if (bytesAvailable == 0)
                {
                    return Array.Empty<byte>();
                }

                if (bytesAvailable <= bytesToRead)
                {
                    data = lockedSources.Select(x => x.ReadAll()).ToArray();
                }
                else
                {
                    data = new byte[lockedSources.Count][];
                    if (MixDataFromDifferentSources)
                    {
                        lockedSources = lockedSources.OrderBy(x => x.BytesInBuffer).ToList();
                        for (int i = 0; i < lockedSources.Count; i++)
                        {
                            data[i] = lockedSources[i].ReadUpTo((bytesToRead - data.Where(x => x != null).Sum(x => x.Length)) / (lockedSources.Count - i));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < lockedSources.Count; i++)
                        {
                            data[i] = lockedSources[i].ReadUpTo(bytesToRead - data.Where(x => x != null).Sum(x => x.Length));
                        }
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
    }
}
