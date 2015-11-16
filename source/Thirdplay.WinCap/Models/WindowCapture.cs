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
        /// 画面キャプチャ
        /// </summary>
        private ScreenCapture screenCapture = new ScreenCapture();

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
            Rectangle rect = WindowHelper.GetWindowBounds(handle);
            return screenCapture.Capture(rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}
