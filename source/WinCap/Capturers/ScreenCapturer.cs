using System.Drawing;

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
            return ScreenHelper.CaptureScreen(ScreenHelper.GetFullScreenBounds());
        }
    }
}
