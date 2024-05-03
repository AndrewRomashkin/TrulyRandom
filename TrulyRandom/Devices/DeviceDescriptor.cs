namespace TrulyRandom.Devices;

/// <summary>
/// Describes a DirectShow device.
/// </summary>
public class DeviceDescriptor
{
    /// <summary>
    /// Name of the device.
    /// </summary>
    public string Name;
    /// <summary>
    /// Unique device path.
    /// </summary>
    public string MonikerString;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceDescriptor" /> class
    /// </summary>
    /// <param name="name">Device name</param>
    /// <param name="monikerString">Unique device path</param>
    public DeviceDescriptor(string name, string monikerString)
    {
        Name = name;
        MonikerString = monikerString;
    }
}
