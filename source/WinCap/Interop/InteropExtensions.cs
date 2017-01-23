using System;
using System.Runtime.InteropServices;
using System.Text;
using WinCap.Interop.Win32;
using Rectangle = System.Drawing.Rectangle;

namespace WinCap.Interop
{
    public static class InteropExtensions
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
        /// <returns>ウィンドウの範囲</returns>
        public static Rectangle GetWindowBounds(IntPtr handle)
        {
            // Vista以降の場合、DWMでウィンドウサイズを取得
            var rect = new RECT();
            if (Environment.OSVersion.Version.Major >= 6)
            {
                if (Dwmapi.DwmGetWindowAttribute(handle, (int)DWMWA.EXTENDED_FRAME_BOUNDS, ref rect, Marshal.SizeOf(typeof(RECT))) == 0)
                {
                    var rectangle = new Rectangle(
                        rect.left, rect.top,
                        rect.right - rect.left, rect.bottom - rect.top);
                    if (rectangle.Width > 0 && rectangle.Height > 0)
                    {
                        // 高DPI対応
                        var dpi = PerMonitorDpi.GetDpi(handle);
                        rectangle.X = (int)Math.Round(rectangle.X * dpi.ScaleX);
                        rectangle.Y = (int)Math.Round(rectangle.Y * dpi.ScaleY);
                        rectangle.Width = (int)Math.Round(rectangle.Width * dpi.ScaleX);
                        rectangle.Height = (int)Math.Round(rectangle.Height * dpi.ScaleY);
                        return rectangle;
                    }
                }
            }

            // ウィンドウサイズの取得
            if (User32.GetWindowRect(handle, out rect))
            {
                return new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
            }
            return Rectangle.Empty;
        }
    }
}
