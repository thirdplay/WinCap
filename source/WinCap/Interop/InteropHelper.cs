using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
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
        /// 指定ウィンドウハンドルのウィンドウサイズを取得します。
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        /// <param name="isHighDpi">高DPI対応を処理するかどうか</param>
        /// <returns>ウィンドウサイズ</returns>
        public static Rectangle GetWindowSize(IntPtr handle, bool isHighDpi = true)
        {
            try
            {
                var result = GetWindowSize(handle);
                if (result != Rectangle.Empty)
                {
                    // 最大化状態のウィンドウの場合
                    var style = User32.GetWindowLong(handle, GWL.STYLE);
                    if ((style & (int)WS.MAXIMIZE) == (int)WS.MAXIMIZE)
                    {
                        // スクリーン取得
                        var point = new Point(
                            result.Left + result.Width / 2, result.Top + result.Height / 2);
                        var screen = Screen.AllScreens
                            .Where(x => x.Bounds.Contains(point))
                            .FirstOrDefault();

                        // スクリーンの作業領域を設定する
                        if (screen != null)
                        {
                            result = screen.WorkingArea;
                        }
                    }

                    // 高DPI対応
                    if (isHighDpi)
                    {
                        result = result.ToLogicalPixel(PerMonitorDpi.GetDpi(handle));
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Application.ReportException(null, ex, false);
                return Rectangle.Empty;
            }
        }

        /// <summary>
        /// 指定ウィンドウハンドルのウィンドウサイズを取得します。
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        /// <returns>ウィンドウサイズ</returns>
        private static Rectangle GetWindowSize(IntPtr handle)
        {
            // Vista以降の場合、DWMでウィンドウサイズを取得
            var rect = new RECT();
            if (Environment.OSVersion.Version.Major >= 6)
            {
                if (Dwmapi.DwmGetWindowAttribute(handle, (int)DWMWA.EXTENDED_FRAME_BOUNDS, ref rect, Marshal.SizeOf(typeof(RECT))) == 0)
                {
                    var rectangle = new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
                    if (rectangle.Width > 0 && rectangle.Height > 0)
                    {
                        return rectangle;
                    }
                }
            }
            if (User32.GetWindowRect(handle, out rect))
            {
                return new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
            }
            return Rectangle.Empty;
        }
    }
}
