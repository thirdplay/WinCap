using System.Runtime.InteropServices;

namespace WinCap.Win32
{
    /// <summary>
    /// 矩形構造体
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }
}
