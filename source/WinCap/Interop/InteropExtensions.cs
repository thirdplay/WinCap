using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using WinCap.Interop.Win32;
using Rectangle = System.Drawing.Rectangle;

namespace WinCap.Interop
{
    public static class InteropExtensions
    {
        /// <summary>
        /// 現在の <see cref="T:System.Windows.Media.Visual"/> からDPI倍率を取得します。
        /// </summary>
        /// <param name="visual"></param>
        /// <returns>
        /// X軸およびY軸それぞれのDPI倍率を表す <see cref="global::ThisAssembly:System.Windows.Point"/>構造体。
        /// 取得に失敗した場合、(1.0, 1.0) を返します。
        /// </returns>
        public static Point GetDpiScaleFactor(this Visual visual)
        {
            var source = PresentationSource.FromVisual(visual);
            if (source?.CompositionTarget != null)
            {
                return new Point(
                    source.CompositionTarget.TransformToDevice.M11,
                    source.CompositionTarget.TransformToDevice.M22);
            }
            return new Point(1.0, 1.0);
        }

        /// <summary>
        /// ウィンドウクラス名を取得します。
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        /// <returns>クラス名</returns>
        public static string GetClassName(this IntPtr handle)
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
        public static Rectangle GetWindowBounds(this IntPtr handle)
        {
            // Vista以降の場合、DWMでウィンドウサイズを取得
            RECT rect = new RECT();
            if (Environment.OSVersion.Version.Major >= 6)
            {
                if (Dwmapi.DwmGetWindowAttribute(handle, (int)DWMWA.EXTENDED_FRAME_BOUNDS, ref rect, Marshal.SizeOf(typeof(RECT))) == 0)
                {
                    Rectangle rectangle = new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
                    if (rectangle.Width > 0 && rectangle.Height > 0)
                    {
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
