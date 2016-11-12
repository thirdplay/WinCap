using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using WinCap.Interop;

namespace WinCap.Models
{
    /// <summary>
    /// アンマネージ関数の補助機能を提供します。
    /// </summary>
    internal static class InteropHelper
    {
        /// <summary>
        /// ウィンドウクラス名を取得します。
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        /// <returns>クラス名</returns>
        public static string GetClassName(IntPtr handle)
        {
            StringBuilder builder = new StringBuilder(256);
            if (NativeMethods.GetClassName(handle, builder, builder.Capacity))
            {
                return builder.ToString();
            }
            return "";
        }

        /// <summary>
        /// 指定ウィンドウハンドルの範囲を取得します。
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        /// <returns>ウィンドウの範囲</returns>
        public static Rectangle GetWindowBounds(IntPtr handle)
        {
            // Vista以降の場合、DWMでウィンドウサイズを取得
            RECT rect = new RECT();
            if (Environment.OSVersion.Version.Major >= 6)
            {
                if (NativeMethods.DwmGetWindowAttribute(handle, (int)DWMWA.EXTENDED_FRAME_BOUNDS, ref rect, Marshal.SizeOf(typeof(RECT))) == 0)
                {
                    Rectangle rectangle = new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
                    if (rectangle.Width > 0 && rectangle.Height > 0)
                    {
                        return rectangle;
                    }
                }
            }

            // ウィンドウサイズの取得
            if (NativeMethods.GetWindowRect(handle, out rect))
            {
                return new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
            }
            return Rectangle.Empty;
        }
    }
}
