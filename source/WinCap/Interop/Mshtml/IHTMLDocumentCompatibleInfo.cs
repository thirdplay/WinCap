using System;
using System.Runtime.InteropServices;

namespace WinCap.Interop.Mshtml
{
    [Guid("3051041A-98B5-11CF-BB82-00AA00BDCE0B")]
    [TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable)]
    public interface IHTMLDocumentCompatibleInfo
    {
        [DispId(1001)]
        string userAgent { get; }
        [DispId(1002)]
        string version { get; }
    }
}
