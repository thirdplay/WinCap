using System;
using System.Runtime.InteropServices;

namespace WinCap.Interop.Win32
{
    public static class Dwmapi
    {
        /// <summary>
        /// ウィンドウの属性情報を返す。
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="dwAttribute">属性の識別子</param>
        /// <param name="pvAttribute">属性情報の参照</param>
        /// <param name="cbAttribute">属性情報のサイズ</param>
        /// <returns>成功なら0、失敗なら0以外を返します。</returns>
        [DllImport("Dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hWnd, int dwAttribute, ref RECT pvAttribute, int cbAttribute);
    }
}
