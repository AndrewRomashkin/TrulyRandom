using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using TrulyRandom.Models;

namespace TrulyRandom.Modules
{
    public class Buffer : Module
    {
        public Module[] Sources { get; set; } = new Module[0];
        public int BufferFileSize { get; set; } = 1000000;
        public int MaxFilesToStore { get; set; } = 0;
        public int MinBytesInBuffer { get; set; } = 100000;
        public string BufferDirectory = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "RandomnessBuffer";
        public bool MixDataFromDifferentSources { get; set; } = true;

        public Buffer()
        {
            thread = new Thread(WorkerThread);
            thread.Name = Name;
        }

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

                SaveToBuffer();
                LoadFromBuffer();
                Thread.Sleep(1);
            }
            Disposed = true;
        }

        object directoryLock = new object();
        DateTime nextWriteCheck = DateTime.Now;
        void SaveToBuffer()
        {
            if (DateTime.Now < nextWriteCheck || BytesInBuffer < MinBytesInBuffer + BufferFileSize)
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
                        List<int> files = new List<int>();
                        foreach (string file in Directory.GetFiles(BufferDirectory))
                        {
                            int number;
                            if (int.TryParse(Path.GetFileName(file), out number))
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
        void LoadFromBuffer()
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
                    List<int> files = new List<int>();
                    foreach (string file in Directory.GetFiles(BufferDirectory))
                    {
                        int number;
                        if (int.TryParse(Path.GetFileName(file), out number))
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

        public void ClearBufferedData()
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
                    int number;
                    if (int.TryParse(Path.GetFileName(file), out number))
                    {
                        result++;
                    }
                }
                return result;
            }
        }

        byte[] ReadDataFromSources()
        {
            Module[] sources = Sources;
            if (sources.Length == 0)
            {
                return new byte[0];
            }

            List<Module> lockedSources = new List<Module>();
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
                    return new byte[0];
                }

                int bytesToRead = BufferSize - BytesInBuffer;
                int bytesAvailable = lockedSources.Sum(x => x.BytesInBuffer);
                byte[][] data;

                if (bytesAvailable == 0)
                {
                    return new byte[0];
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
                    return new byte[0];
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
                return new byte[0];
            }
            finally
            {
                foreach (Module source in lockedSources)
                {
                    Monitor.Exit(source.Sync);
                }
            }
        }

        public void AddSource(Module source)
        {
            if (Sources.Contains(source))
            {
                return;
            }
            Sources = Sources.Append(source);
        }

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
