using System;
using System.Drawing;
using WinCap.Interop;
using WinCap.Models;

namespace WinCap.Capturable
{
    /// <summary>
    /// コントロールをキャプチャーする機能を提供します。
    /// </summary>
    public class CapturableControl
    {
        /// <summary>
        /// 画面をキャプチャする機能
        /// </summary>
        private CapturableScreen _screen;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="screen">画面キャプチャ</param>
        public CapturableControl(CapturableScreen screen)
        {
            _screen = screen;
        }

        /// <summary>
        /// アクティブなコントロールをキャプチャします。
        /// </summary>
        /// <returns>ビットマップ</returns>
        public Bitmap CaptureActiveControl()
        {
            return CaptureControl(NativeMethods.GetForegroundWindow());
        }

        /// <summary>
        /// 指定ハンドルのコントロールをキャプチャします。
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        /// <returns>ビットマップ</returns>
        public Bitmap CaptureControl(IntPtr handle)
        {
            return _screen.CaptureBounds(WindowHelper.GetWindowBounds(handle));
        }
    }
}
