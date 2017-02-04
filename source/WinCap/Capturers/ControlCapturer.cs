using System;
using System.Drawing;
using WinCap.Interop;
using WinCap.Interop.Win32;
using WinCap.Models;

namespace WinCap.Capturers
{
    /// <summary>
    /// コントロールをキャプチャーする機能を提供します。
    /// </summary>
    public class ControlCapturer
    {
        /// <summary>
        /// 画面キャプチャ
        /// </summary>
        private readonly ScreenCapturer capturer = new ScreenCapturer();

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
            return capturer.CaptureBounds(InteropExtensions.GetWindowBounds(handle, false));
        }
    }
}
