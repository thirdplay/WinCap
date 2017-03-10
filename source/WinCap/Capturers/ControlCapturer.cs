using System;
using System.Drawing;
using WinCap.Interop;
using WinCap.Interop.Win32;

namespace WinCap.Capturers
{
    /// <summary>
    /// コントロールをキャプチャーする機能を提供します。
    /// </summary>
    public class ControlCapturer
    {
        /// <summary>
        /// アクティブなコントロールをキャプチャします。
        /// </summary>
        /// <returns>ビットマップ</returns>
        public Bitmap CaptureActiveControl()
        {
            return CaptureControl(User32.GetForegroundWindow());
        }

        /// <summary>
        /// 指定ハンドルのコントロールをキャプチャします。
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        /// <returns>ビットマップ</returns>
        public Bitmap CaptureControl(IntPtr handle)
        {
            return ScreenHelper.CaptureScreen(InteropExtensions.GetWindowBounds(handle, false));
        }
    }
}
