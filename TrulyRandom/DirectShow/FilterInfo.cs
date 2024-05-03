using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

// Source: AForge library
namespace TrulyRandom.DirectShow;

internal class FilterInfo : IComparable
{
    /// <summary>
    /// Filter name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Filters's moniker string.
    /// </summary>
    public string MonikerString { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterInfo"/> class.
    /// </summary>
    /// <param name="monikerString">Filters's moniker string.</param>
    public FilterInfo(string monikerString)
    {
        MonikerString = monikerString;
        Name = GetName(monikerString);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FilterInfo"/> class.
    /// </summary>
    /// <param name="moniker">Filter's moniker object.</param>
    internal FilterInfo(IMoniker moniker)
    {
        MonikerString = GetMonikerString(moniker);
        Name = GetName(moniker);
    }

    /// <summary>
    /// Compare the object with another instance of this class.
    /// </summary>
    /// <param name="value">Object to compare with.</param>
    /// <returns>A signed number indicating the relative values of this instance and <b>value</b>.</returns>
    public int CompareTo(object value)
    {
        FilterInfo f = (FilterInfo)value;

        if (f == null)
        {
            return 1;
        }

        return string.Compare(Name, f.Name, StringComparison.Ordinal);
    }

    /// <summary>
    /// Create an instance of the filter.
    /// </summary>
    /// <param name="filterMoniker">Filter's moniker string.</param>
    /// <returns>Returns filter's object, which implements <b>IBaseFilter</b> interface.</returns>
    /// <remarks>The returned filter's object should be released using <b>Marshal.ReleaseComObject()</b>.</remarks>
    public static object CreateFilter(string filterMoniker)
    {
        // filter's object
        object filterObject = null;
        int n = 0;

        // bind context and moniker objects
        // create bind context
        if (Win32.CreateBindCtx(0, out IBindCtx bindCtx) == 0)
        {
            // convert moniker`s string to a moniker
            if (Win32.MkParseDisplayName(bindCtx, filterMoniker, ref n, out IMoniker moniker) == 0)
            {
                // get device base filter
                Guid filterId = typeof(IBaseFilter).GUID;
                moniker.BindToObject(null, null, ref filterId, out filterObject);

                Marshal.ReleaseComObject(moniker);
            }
            Marshal.ReleaseComObject(bindCtx);
        }
        return filterObject;
    }

    /// <summary>
    /// Get moniker string of the moniker.
    /// </summary>
    private static string GetMonikerString(IMoniker moniker)
    {
        moniker.GetDisplayName(null, null, out string str);
        return str;
    }

    /// <summary>
    /// Get filter name represented by the moniker.
    /// </summary>
    private static string GetName(IMoniker moniker)
    {
        object bagObj = null;
        try
        {
            Guid bagId = typeof(IPropertyBag).GUID;
            // get property bag of the moniker
            moniker.BindToStorage(null, null, ref bagId, out bagObj);
            IPropertyBag bag = (IPropertyBag)bagObj;

            // read FriendlyName
            object val = "";
            int hr = bag.Read("FriendlyName", ref val, IntPtr.Zero);
            if (hr != 0)
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            // get it as string
            string ret = (string)val;
            if ((ret == null) || (ret.Length < 1))
            {
                throw new ApplicationException();
            }

            return ret;
        }
#pragma warning disable CA1031 // Do not catch general exception typesption)
        catch (Exception)
        {
            return "";
        }
#pragma warning restore CA1031 // Do not catch general exception types
        finally
        {
            // release all COM objects
            if (bagObj != null)
            {
                Marshal.ReleaseComObject(bagObj);
            }
        }
    }

    /// <summary>
    /// Get filter name represented by the moniker string.
    /// </summary>
    private static string GetName(string monikerString)
    {
        string name = "";
        int n = 0;

        // create bind context
        if (Win32.CreateBindCtx(0, out IBindCtx bindCtx) == 0)
        {
            // convert moniker`s string to a moniker
            if (Win32.MkParseDisplayName(bindCtx, monikerString, ref n, out IMoniker moniker) == 0)
            {
                // get device name
                name = GetName(moniker);

                Marshal.ReleaseComObject(moniker);
            }
            Marshal.ReleaseComObject(bindCtx);
        }
        return name;
    }
}
