using MetroRadiance.Utilities;
using System;
using System.Drawing;
using WinCap.Interop.Win32;
using WinCap.Models;

namespace WinCap.Capturers
{
    /// <summary>
    /// 画面をキャプチャする機能を提供します。
    /// </summary>
    public class ScreenCapturer
    {
        /// <summary>
        /// 画面全体をキャプチャする。
        /// </summary>
        /// <returns>ビットマップ</returns>
        public Bitmap CaptureFullScreen()
        {
            return CaptureBounds(ScreenHelper.GetFullScreenBounds());
        }

        /// <summary>
        /// 指定範囲の画面をキャプチャする。
        /// </summary>
        /// <param name="bounds">範囲</param>
        /// <returns>ビットマップ</returns>
        public Bitmap CaptureBounds(Rectangle bounds)
        {
            var bitmap = new Bitmap(bounds.Width, bounds.Height);
            var screenDC = User32.GetDC(IntPtr.Zero);
            using (Disposable.Create(() => User32.ReleaseDC(IntPtr.Zero, screenDC)))
            using (Graphics g = Graphics.FromImage(bitmap))
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
    }
}
