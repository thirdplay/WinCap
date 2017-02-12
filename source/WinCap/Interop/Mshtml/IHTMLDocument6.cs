using mshtml;
using System;
using System.Runtime.InteropServices;

namespace WinCap.Interop.Mshtml
{
    [Guid("30510417-98B5-11CF-BB82-00AA00BDCE0B")]
    [TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable)]
    public interface IHTMLDocument6
    {
        [DispId(1103)]
        IHTMLDocumentCompatibleInfoCollection compatible { get; }
        [DispId(1104)]
        dynamic documentMode { get; }
        [DispId(-2147412012)]
        dynamic onstorage { get; set; }
        [DispId(-2147412011)]
        dynamic onstoragecommit { get; set; }

        [DispId(1107)]
        IHTMLElement2 getElementById(string bstrId);
        [DispId(1109)]
        void updateSettings();
    }
}
