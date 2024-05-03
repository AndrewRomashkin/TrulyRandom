using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

// Source: AForge library
namespace TrulyRandom.DirectShow;

[ComImport, Guid("29840822-5B84-11D0-BD3B-00A0C911CE86"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface ICreateDevEnum
{
    /// <summary>
    /// Creates a class enumerator for a specified device category.
    /// </summary>
    /// <param name="type">Specifies the class identifier of the device category.</param>
    /// <param name="enumMoniker">Address of a variable that receives an <b>IEnumMoniker</b> interface pointer</param>
    /// <param name="flags">Bitwise combination of zero or more flags. If zero, the method enumerates every filter in the category.</param>
    /// <returns>Return's <b>HRESULT</b> error code.</returns>
    [PreserveSig]
    int CreateClassEnumerator([In] ref Guid type, [Out] out IEnumMoniker enumMoniker, [In] int flags);
}
