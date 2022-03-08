using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using TrulyRandom.DirectShow;

// Source: AForge library
namespace TrulyRandom.Devices
{
    /// <summary>
    /// Basic class for Direct Show devices
    /// </summary>
    public abstract class DirectShowDevice : IDisposable
    {
        /// <summary>
        /// Determines whether device should be running
        /// </summary>
        bool run = false;
        /// <summary>
        /// Show whether device is running
        /// </summary>
        bool running = false;
        /// <summary>
        /// Determines whether device should be disposed
        /// </summary>
        bool dispose = false;
        /// <summary>
        /// Show whether device is disposed
        /// </summary>
        bool disposed = false;
        /// <summary>
        /// Last exception thrown within the main loop
        /// </summary>
        Exception lastException = null;
        /// <summary>
        /// Unique device path
        /// </summary>
        readonly string monikerString;
        /// <summary>
        /// Main thread of the object
        /// </summary>
        protected Thread thread = null;

        /// <summary>
        /// Fires when new data from the device is recieved
        /// </summary>
        public event Action<byte[]> NewData;
        /// <summary>
        /// Fires when new exception is thrown within the main loop
        /// </summary>
        public event Action<Exception> NewException;
        /// <summary>
        /// Shows whether device is running
        /// </summary>
        public bool Running => running;
        /// <summary>
        /// Shows whether device is disposed
        /// </summary>
        public bool Disposed => disposed;
        /// <summary>
        /// Last exception thrown within the main loop
        /// </summary>
        public Exception LastException => lastException;
        /// <summary>
        /// Shows if the device keeps returning same samples. It happens when audio input is silent or camera image is frozen
        /// </summary>
        public bool Still { get; private set; } = false;
        /// <summary>
        /// Determines whether repeating samples should be discarded
        /// </summary>
        public bool FilterOutput { get; set; } = true;
        /// <summary>
        /// Return count of samples receved
        /// </summary>
        public double SamplesRecieved { get; private set; } = 0;
        /// <summary>
        /// Return actual average sample rate  
        /// </summary>
        public double ActualSampleRate { get; private set; } = 0;
        /// <summary>
        /// Return actual average bytes per sample 
        /// </summary>
        public int ActualBytesPerSample { get; private set; } = 0;
        /// <summary>
        /// Return actual average bytes per second
        /// </summary>
        public int ActualBytesPerSecond => (int)ActualSampleRate * ActualBytesPerSample;
        /// <summary>
        /// Device name
        /// </summary>
        public string Name { get; protected set; } = null;

        /// <summary>
        /// Starts capture
        /// </summary>
        public void Start()
        {
            if (dispose)
            {
                return;
            }
            run = true;
            if ((thread.ThreadState & ThreadState.Unstarted) == ThreadState.Unstarted)
            {
                thread.Start();
            }
        }

        /// <summary>
        /// Stops capture
        /// </summary>
        public void Stop()
        {
            run = false;
        }

        /// <summary>
        /// Releases all resources used by this object
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            dispose = true;
            if (thread == null || (thread.ThreadState & ThreadState.Unstarted) == ThreadState.Unstarted)
            {
                ReleaseDevice(monikerString);
                disposed = true;
            }
        }

        #region Static members

        /// <summary>
        /// Stores a list of moniker strings of all devices currently used by any instance of this object
        /// </summary>
        static readonly List<string> devicesInUse = new();

        /// <summary>
        /// Returns all device descriptors of given type currently available in the system
        /// </summary>
        protected static DeviceDescriptor[] GetAvailableDevices(Guid deviceType)
        {
            List<DeviceDescriptor> result = new();
            FilterInfoCollection devices = new(deviceType);
            lock (devicesInUse)
            {
                for (int i = 0; i < devices.Count; i++)
                {
                    if (!devicesInUse.Contains(devices[i].MonikerString))
                    {
                        result.Add(new DeviceDescriptor(devices[i].Name, devices[i].MonikerString));
                    }
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Locks specified device so it can't be accessed by another instance
        /// </summary>
        /// <param name="monikerString">Moniker string of the device to be locked</param>
        /// <returns><c>True</c> is device was locked successfully</returns>
        static bool LockDevice(string monikerString)
        {
            lock (devicesInUse)
            {
                if (devicesInUse.Contains(monikerString))
                {
                    return false;
                }
                devicesInUse.Add(monikerString);
            }
            return true;
        }

        /// <summary>
        /// Unocks specified device so it can be accessed by another instance
        /// </summary>
        /// <param name="monikerString">Moniker string of the device to be unlocked</param>
        static void ReleaseDevice(string monikerString)
        {
            lock (devicesInUse)
            {
                devicesInUse.Remove(monikerString);
            }
        }

        /// <summary>
        /// Finds first operational device of the given type. If none found returns <c>null</c>
        /// </summary>
        /// <param name="timeout">Timeout for waiting for data from the device</param>
        /// <param name="deviceType">Device type</param>
        /// <param name="deviceTypeGuid">Device type GUID</param>
        /// <returns>Descriptor of the first operational device found</returns>
        protected static DeviceDescriptor FindFirstOperationalDevice(int timeout, Type deviceType, Guid deviceTypeGuid)
        {
            foreach (DeviceDescriptor deviceDescriptor in GetAvailableDevices(deviceTypeGuid))
            {
                DirectShowDevice testDevice = null;
                try
                {
                    testDevice = deviceType.GetConstructor(new Type[] { typeof(DeviceDescriptor) }).Invoke(new object[] { deviceDescriptor }) as DirectShowDevice;
                    testDevice.FilterOutput = true;
                    testDevice.Start();
                    DateTime startTime = DateTime.Now;
                    while (!startTime.WasAgo(TimeSpan.FromMilliseconds(timeout)) && testDevice.SamplesRecieved < 2 && !testDevice.Still)
                    {
                        Thread.Sleep(10);
                    }
                    if (testDevice.SamplesRecieved >= 2 && !testDevice.Still)
                    {
                        testDevice.Dispose();
                        while (!testDevice.Disposed)
                        {
                            Thread.Sleep(10);
                        }
                        return deviceDescriptor;
                    }
                }
                catch { }
                finally
                {
                    testDevice?.Dispose();
                }
            }
            return null;
        }

        #endregion Static members

        /// <summary>
        /// Returns a static lock object for all instances of this type
        /// </summary>
        protected abstract object GetTypeSync();

        /// <summary>
        /// Creates new DirectShow device by searching through available devices and attempting to get alternating samples from those
        /// </summary>
        /// <exception cref="DeviceNotFoundException">Thrown when no suitable devices found. If device is found and then goes offline object will keep trying to establish connection and no exception will be thrown</exception>
        /// <param name="autoSearchTimeout">Timeout for waiting for data from the device</param>
        public DirectShowDevice(int autoSearchTimeout)
        {
            lock (GetTypeSync())
            {
                DeviceDescriptor deviceDescriptor = FindFirstOperationalDevice(autoSearchTimeout, GetType(), GetDeviceType());
                if (deviceDescriptor == null)
                {
                    throw new DeviceNotFoundException("No operational devices found");
                }
                Name = deviceDescriptor.Name;
                monikerString = deviceDescriptor.MonikerString;
                Initialize();
            }
        }

        /// <summary>
        /// Creates new DirectShow device from given <see cref="DeviceDescriptor"/>. If device is unavailable object will keep trying to establish connection and no exception will be thrown
        /// </summary>
        public DirectShowDevice(DeviceDescriptor deviceDescriptor)
        {
            Name = deviceDescriptor.Name;
            monikerString = deviceDescriptor.MonikerString;
            Initialize();
        }

        /// <summary>
        /// Creates new DirectShow device from given name and moniker string. If device is unavailable object will keep trying to establish connection and no exception will be thrown
        /// </summary>
        public DirectShowDevice(string name, string monikerString)
        {
            Name = name;
            this.monikerString = monikerString;
            Initialize();
        }

        void Initialize()
        {
            if (!LockDevice(monikerString))
            {
                throw new ArgumentException("This device is already in use");
            }
            thread = new(new ThreadStart(WorkerThread));
            thread.IsBackground = true;
            thread.Name = GetThreadName();
        }

        // Members specific to the device type
        private protected abstract Guid GetDeviceType();
        private protected abstract string GetThreadName();
        private protected abstract AMMediaType GetAMMediaType();
        private protected abstract Guid GetMediaType();
        private protected abstract void GetAndConfigurePinCapabilities(ICaptureGraphBuilder2 graphBuilder, IBaseFilter baseFilter);

        /// <summary>
        /// Main loop
        /// </summary>
        void WorkerThread()
        {
            Grabber grabber = new();
            object captureGraphObject = null;
            object graphObject = null;
            object grabberObject = null;
            object sourceObject = null;

            //Interfaces
            ICaptureGraphBuilder2 captureGraph = null;
            IFilterGraph2 graph = null;
            IBaseFilter sourceBase = null;
            IBaseFilter grabberBase = null;
            ISampleGrabber sampleGrabber = null;
            IMediaControl mediaControl = null;
            IMediaEventEx mediaEvent = null;

            while (!dispose)
            {
                try
                {
                    while (!run && !dispose)
                    {
                        Thread.Sleep(10);
                    }
                    if (run && !dispose)
                    {
                        //Get type of capture graph builder
                        Type type = Type.GetTypeFromCLSID(Clsid.CaptureGraphBuilder2);
                        if (type == null)
                        {
                            throw new ApplicationException("Failed creating capture graph builder");
                        }

                        //Create capture graph builder
                        captureGraphObject = Activator.CreateInstance(type);
                        captureGraph = (ICaptureGraphBuilder2)captureGraphObject;

                        //Get type of filter graph
                        type = Type.GetTypeFromCLSID(Clsid.FilterGraph);
                        if (type == null)
                        {
                            throw new ApplicationException("Failed creating filter graph");
                        }

                        //Create filter graph
                        graphObject = Activator.CreateInstance(type);
                        graph = (IFilterGraph2)graphObject;

                        //Set filter graph to the capture graph builder
                        captureGraph.SetFiltergraph((IGraphBuilder)graph);

                        //Create source device's object
                        sourceObject = FilterInfo.CreateFilter(monikerString);
                        if (sourceObject == null)
                        {
                            throw new ApplicationException("Failed creating device object for moniker");
                        }

                        //Get base filter interface of source device
                        sourceBase = (IBaseFilter)sourceObject;

                        //Get type of sample grabber
                        type = Type.GetTypeFromCLSID(Clsid.SampleGrabber);
                        if (type == null)
                        {
                            throw new ApplicationException("Failed creating sample grabber");
                        }

                        //Create sample grabber used for video capture
                        grabberObject = Activator.CreateInstance(type);
                        sampleGrabber = (ISampleGrabber)grabberObject;
                        grabberBase = (IBaseFilter)grabberObject;

                        //Add source and grabber filters to graph
                        graph.AddFilter(sourceBase, "source");
                        graph.AddFilter(grabberBase, "grabber");

                        //Set media type
                        sampleGrabber.SetMediaType(GetAMMediaType());

                        //Configure video sample grabber
                        grabber.NewData += Grabber_NewData;
                        sampleGrabber.SetBufferSamples(false);
                        sampleGrabber.SetOneShot(false);
                        sampleGrabber.SetCallback(grabber, 1);

                        //Configure pins
                        GetAndConfigurePinCapabilities(captureGraph, sourceBase);

                        //Render capture pin
                        captureGraph.RenderStream(PinCategory.Capture, GetMediaType(), sourceBase, null, grabberBase);

                        //Get media control
                        mediaControl = (IMediaControl)graphObject;

                        //Get media events' interface
                        mediaEvent = (IMediaEventEx)graphObject;

                        //Run
                        mediaControl.Run();
                        while (!dispose && run)
                        {
                            if (mediaEvent != null)
                            {

                                if (mediaEvent.GetEvent(out DsEvCode code, out IntPtr p1, out IntPtr p2, 0) >= 0)
                                {
                                    mediaEvent.FreeEventParams(code, p1, p2);

                                    if (code == DsEvCode.DeviceLost)
                                    {
                                        running = false;
                                        break;
                                    }
                                    else
                                    {
                                        running = true;
                                    }
                                }
                            }
                        }

                        mediaControl.Stop();
                        running = false;
                        lastDataRecieved = null;
                    }
                    if (!run && !dispose)
                    {
                        Thread.Sleep(10);
                    }
                }
                catch (Exception e)
                {
                    lastException = e;
                    NewException?.Invoke(e);
                    Thread.Sleep(5000);
                }
                finally
                {
                    //Release all objects
                    if (graphObject != null)
                    {
                        Marshal.ReleaseComObject(graphObject);
                    }
                    if (sourceObject != null)
                    {
                        Marshal.ReleaseComObject(sourceObject);
                    }
                    if (grabberObject != null)
                    {
                        grabber.NewData -= NewData;
                        Marshal.ReleaseComObject(grabberObject);
                    }
                    if (captureGraphObject != null)
                    {
                        Marshal.ReleaseComObject(captureGraphObject);
                    }
                    ReleaseDevice(monikerString);
                }
            }
            disposed = true;
        }

        byte[] lastData = null;
        readonly object grabberSync = new();
        readonly List<double> sampleIntervals = new();
        readonly List<int> sampleSizes = new();
        DateTime? lastDataRecieved = null;

        /// <summary>
        /// Handler for the data from the device
        /// </summary>
        /// <param name="data">Sample data</param>
        void Grabber_NewData(byte[] data)
        {
            if (!Monitor.TryEnter(grabberSync, 1) || !running)
            {
                return;
            }

            try
            {
                if (lastData != null)
                {
                    //Filtering still images
                    if (!FilterOutput)
                    {
                        Still = false;
                    }
                    else
                    {
                        bool still = true;
                        for (int i = 0; i < data.Length; i++)
                        {
                            if (i >= lastData.Length || data[i] != lastData[i])
                            {
                                still = false;
                                break;
                            }
                        }

                        Still = still;
                    }
                }

                //First frame is discarded: it can be still
                if (lastDataRecieved != null && !Still)
                {
                    sampleSizes.Add(data.Length);
                    sampleIntervals.Add((DateTime.Now - lastDataRecieved.Value).TotalMilliseconds);
                    ActualSampleRate = 1000 / sampleIntervals.Average();
                    ActualBytesPerSample = (int)sampleSizes.Average();
                    SamplesRecieved++;
                }

                lastDataRecieved = DateTime.Now;
                lastData = data;

                if (NewData != null && !Still && !dispose)
                {
                    NewData(data);
                }
            }
            finally
            {
                Monitor.Exit(grabberSync);
            }
        }

        /// <summary>
        /// Finalizes the object
        /// </summary>
        ~DirectShowDevice()
        {
            Dispose();
        }
    }
}