using System;
using System.Drawing;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows.Forms;
using WinCap.Interop;
using WinCap.Interop.Win32;

namespace WinCap.Capturers
{
    /// <summary>
    /// 画面の補助機能を提供します。
    /// </summary>
    internal static class ScreenHelper
    {
        /// <summary>
        /// 画面全体の範囲を返却します。
        /// </summary>
        /// <returns>範囲</returns>
        public static Rectangle GetFullScreenBounds()
        {
            var bounds = new Rectangle();
            foreach (var screen in Screen.AllScreens)
            {
                bounds.X = Math.Min(bounds.X, screen.Bounds.Left);
                bounds.Y = Math.Min(bounds.Y, screen.Bounds.Top);
                bounds.Width = Math.Max(bounds.Width, screen.Bounds.Right);
                bounds.Height = Math.Max(bounds.Height, screen.Bounds.Bottom);
            }

            // オフセット分のサイズ補正
            bounds.Width -= bounds.X;
            bounds.Height -= bounds.Y;

            return bounds;
        }

        /// <summary>
        /// 画面の原点座標を返却します。
        /// </summary>
        /// <returns>画面の原点座標</returns>
        public static Point GetScreenOrigin()
        {
            var bounds = GetFullScreenBounds();
            return bounds.Location;
        }

        /// <summary>
        /// 指定範囲の画面をキャプチャします。
        /// </summary>
        /// <param name="bounds">範囲</param>
        /// <returns>ビットマップ</returns>
        public static Bitmap CaptureScreen(Rectangle bounds)
        {
            var bitmap = new Bitmap(bounds.Width, bounds.Height);
            var screenDC = User32.GetDC(IntPtr.Zero);
            using (Disposable.Create(() => User32.ReleaseDC(IntPtr.Zero, screenDC)))
            using (var g = Graphics.FromImage(bitmap))
            {
                var hDC = g.GetHdc();
                using (Disposable.Create(() => g.ReleaseHdc(hDC)))
                {
                    Gdi32.BitBlt(hDC, 0, 0, bitmap.Width, bitmap.Height,
                        screenDC, bounds.X, bounds.Y, TernaryRasterOperations.SRCCOPY);
                }
            }

            return bitmap;
        }

        /// <summary>
        /// 指定ウィンドウハンドルの範囲をキャプチャします。
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        /// <returns>ビットマップ</returns>
        public static Bitmap CaptureScreen(IntPtr handle)
        {
            return CaptureScreen(InteropHelper.GetWindowSize(handle, false));
        }
    }
}
