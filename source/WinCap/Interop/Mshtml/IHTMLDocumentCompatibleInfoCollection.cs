using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace WinCap.Interop.Mshtml
{
    [DefaultMember("item")]
    [Guid("30510418-98B5-11CF-BB82-00AA00BDCE0B")]
    [TypeLibType(TypeLibTypeFlags.FDual | TypeLibTypeFlags.FDispatchable)]
    public interface IHTMLDocumentCompatibleInfoCollection
    {
        [DispId(1001)]
        int length { get; }

        [DispId(0)]
        IHTMLDocumentCompatibleInfo item(int index);
    }
}
