using System;
using System.Runtime.InteropServices;

namespace WinCap.Interop.Win32
{
    public static class Oleacc
    {
        [DllImport("oleacc.dll")]
        public static extern int ObjectFromLresult(UIntPtr lResult, ref Guid riid, Int32 wParam, ref MSHTML.IHTMLDocument2 ppvObject);
    }
}
