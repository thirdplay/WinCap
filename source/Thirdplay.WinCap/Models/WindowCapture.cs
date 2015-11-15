using System;
using System.Drawing;
using WinCap.Win32;

namespace WinCap.Models
{
    /// <summary>
    /// ウィンドウをキャプチャーするクラス。
    /// </summary>
    public class WindowCapture
    {
        /// <summary>
        /// アクティブなウィンドウの画像を取得する
        /// </summary>
        /// <returns>アクティブなウィンドウの画像</returns>
        public Bitmap Capture()
        {
            return this.Capture(NativeMethods.GetForegroundWindow());
        }

        /// <summary>
        /// 指定ウィンドウの画像を取得する
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        /// <returns>ウィンドウの画像</returns>
        public Bitmap Capture(IntPtr handle)
        {
            // ウィンドウ領域のサイズを取得
            Rectangle rect = WindowHelper.GetWindowBounds(handle);

            // Bitmapの作成
            Bitmap bmp = new Bitmap(rect.Width, rect.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Bitmapにスクリーンをコピーする
                g.CopyFromScreen(rect.X, rect.Y, 0, 0, rect.Size, CopyPixelOperation.SourceCopy);
                return bmp;
            }
        }
    }
}
