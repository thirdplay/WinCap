using System;
using System.Runtime.InteropServices;

namespace WinCap.Interop.Win32
{
    /// <summary>
    /// 座標を表す構造体。
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        /// <summary>
        /// X座標を取得、設定します。
        /// </summary>
        public int x;

        /// <summary>
        /// Y座標を取得、設定します。
        /// </summary>
        public int y;
    }
}
