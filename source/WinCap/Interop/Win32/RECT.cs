using System;
using System.Runtime.InteropServices;

namespace WinCap.Interop.Win32
{
    /// <summary>
    /// 四角形を表す構造体。
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        /// <summary>
        /// 四角形の上端のX座標を取得、設定します。
        /// </summary>
        public int left;

        /// <summary>
        /// 四角形の上端のY座標を取得、設定します。
        /// </summary>
        public int top;

        /// <summary>
        /// 四角形の底辺のX座標を取得、設定します。
        /// </summary>
        public int right;

        /// <summary>
        /// 四角形の底辺のY座標を取得、設定します。
        /// </summary>
        public int bottom;
    }
}
