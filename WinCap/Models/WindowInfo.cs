using System;
using System.Diagnostics;
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
        /// ウィンドウの位置とサイズ
        /// </summary>
        private Rectangle rectangle = Rectangle.Empty;

        /// <summary>
        /// ウィンドウハンドル
        /// </summary>
        public IntPtr Handle { get; set; } = IntPtr.Zero;

        /// <summary>
        /// ウィンドウの位置とサイズ
        /// </summary>
        public Rectangle Bounds { get { return this.rectangle; } }

        /// <summary>
        /// ウィンドウの座標
        /// </summary>
        public Point Location { get { return this.rectangle.Location; } }

        /// <summary>
        /// ウィンドウのサイズ
        /// </summary>
        public Size Size { get { return this.rectangle.Size; } }

        /// <summary>
        /// ウィンドウの左端のX座標
        /// </summary>
        public int Left { get { return this.rectangle.X; } }

        /// <summary>
        /// ウィンドウの上端のY座標
        /// </summary>
        public int Top { get { return this.rectangle.Y; } }

        /// <summary>
        /// ウィンドウの右端のX座標
        /// </summary>
        public int Right { get { return this.rectangle.Right; } }

        /// <summary>
        /// ウィンドウの下端のY座標
        /// </summary>
        public int Bottom { get { return this.rectangle.Bottom; } }

        /// <summary>
        /// ウィンドウの横幅
        /// </summary>
        public int Width { get { return this.rectangle.Width; } }

        /// <summary>
        /// ウィンドウの高さ
        /// </summary>
        public int Height { get { return this.rectangle.Height; } }

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
                    this.rectangle = new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
                }
            }
            else
            {
                // ウィンドウサイズの取得
                if (NativeMethods.GetWindowRect(handle, ref rect) != 0)
                {
                    this.rectangle = new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
                }
            }
        }
    }
}
