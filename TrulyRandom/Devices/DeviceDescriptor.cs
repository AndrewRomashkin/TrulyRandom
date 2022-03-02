namespace TrulyRandom.Devices
{
    /// <summary>
    /// Describes a DirectShow device
    /// </summary>
    public class DeviceDescriptor
    {
        /// <summary>
        /// Name of the device
        /// </summary>
        public string Name;
        /// <summary>
        /// Unique device path
        /// </summary>
        public string MonikerString;

        public DeviceDescriptor(string name, string monikerString)
        {
            Name = name;
            MonikerString = monikerString;
        }
    }
}
