using System;
using System.Runtime.InteropServices;
using System.Text;
using WinCap.Interop.Win32;
using Rectangle = System.Drawing.Rectangle;

namespace WinCap.Interop
{
    /// <summary>
    /// 相互運用の補助機能を提供します。
    /// </summary>
    public static class InteropHelper
    {
        /// <summary>
        /// ウィンドウクラス名を取得します。
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        /// <returns>クラス名</returns>
        public static string GetClassName(IntPtr handle)
        {
            StringBuilder builder = new StringBuilder(256);
            if (User32.GetClassName(handle, builder, builder.Capacity))
            {
                return builder.ToString();
            }
            return "";
        }

        /// <summary>
        /// 指定ウィンドウハンドルの範囲を取得します。
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        /// <param name="isHighDpi">高DPI対応を処理するかどうか</param>
        /// <returns>ウィンドウの範囲</returns>
        public static Rectangle GetWindowBounds(IntPtr handle, bool isHighDpi = true)
        {
            var result = Rectangle.Empty;

            // Vista以降の場合、DWMでウィンドウサイズを取得
            var rect = new RECT();
            if (Environment.OSVersion.Version.Major >= 6)
            {
                if (Dwmapi.DwmGetWindowAttribute(handle, (int)DWMWA.EXTENDED_FRAME_BOUNDS, ref rect, Marshal.SizeOf(typeof(RECT))) == 0)
                {
                    var rectangle = new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
                    if (rectangle.Width > 0 && rectangle.Height > 0)
                    {
                        result = rectangle;
                    }
                }
            }
            if (result == Rectangle.Empty && User32.GetWindowRect(handle, out rect))
            {
                result = new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
            }

            if (result != Rectangle.Empty)
            {
                // 高DPI対応
                if (isHighDpi)
                {
                    var dpi = PerMonitorDpi.GetDpi(handle);
                    result = result.ToLogicalPixel(dpi);
                }
            }

            return result;
        }
    }
}
