using System;
using System.Drawing;
using System.Windows.Forms;
using WinCap.Utilities.Drawing;
using WinCap.Win32;

namespace WinCap.Models
{
    /// <summary>
    /// 画面全体をキャプチャするクラス。
    /// </summary>
    public class ScreenCapture
    {
        /// <summary>
        /// 画面全体をキャプチャする。
        /// </summary>
        /// <returns>ビットマップ</returns>
        public virtual Bitmap Capture()
        {
            Rectangle scrRect = Screen.AllScreens.GetBounds();
            return Capture(scrRect.X, scrRect.Y, scrRect.Width, scrRect.Height);
        }

        /// <summary>
        /// 指定範囲の画面をキャプチャする。
        /// </summary>
        /// <param name="x">左上のX座標</param>
        /// <param name="y">左上のY座標</param>S
        /// <param name="width">横幅</param>
        /// <param name="height">高さ</param>
        /// <returns>ビットマップ</returns>
        public Bitmap Capture(int x, int y, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            IntPtr screenDC = NativeMethods.GetDC(IntPtr.Zero);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                IntPtr hDC = g.GetHdc();
                NativeMethods.BitBlt(hDC, 0, 0, bmp.Width, bmp.Height,
                    screenDC, x, y, TernaryRasterOperations.SRCCOPY);
                g.ReleaseHdc(hDC);
            }
            NativeMethods.ReleaseDC(IntPtr.Zero, screenDC);

            return bmp;
        }
    }
}
