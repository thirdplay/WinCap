using System;
using System.Drawing;
using System.Runtime.InteropServices;
using WinCap.Win32;

namespace WinCap.Models
{
    /// <summary>
    /// ウィンドウ情報
    /// </summary>
    public class WindowInfo
    {
        /// <summary>
        /// ウィンドウハンドル
        /// </summary>
        public IntPtr Handle { get; set; } = IntPtr.Zero;

        /// <summary>
        /// ウィンドウの座標
        /// </summary>
        public Point Location { get; set; } = Point.Empty;

        /// <summary>
        /// ウィンドウのサイズ
        /// </summary>
        public Size Size { get; set; } = Size.Empty;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        public WindowInfo(IntPtr handle)
        {
            this.Handle = handle;

            // Vista以降の場合、DWMでウィンドウサイズを取得する
            RECT rect = new RECT();
            if (Environment.OSVersion.Version.Major >= 6)
            {
                if (NativeMethods.DwmGetWindowAttribute(handle, (int)DWMWA.EXTENDED_FRAME_BOUNDS, ref rect, Marshal.SizeOf(typeof(RECT))) == 0)
                {
                    this.Location = new Point(rect.left, rect.top);
                    this.Size = new Size(rect.right - rect.left, rect.bottom - rect.top);
                }
            }
            else
            {
                // ウィンドウサイズの取得
                if (NativeMethods.GetWindowRect(handle, ref rect) != 0)
                {
                    this.Location = new Point(rect.left, rect.top);
                    this.Size = new Size(rect.right - rect.left, rect.bottom - rect.top);
                }
            }
        }
    }
}
