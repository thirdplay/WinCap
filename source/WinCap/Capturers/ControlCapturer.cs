using System;
using System.Drawing;
using WinCap.Services;

namespace WinCap.Capturers
{
    /// <summary>
    /// コントロールをキャプチャーする機能を提供します。
    /// </summary>
    public class ControlCapturer : CapturerBase<IntPtr?>
    {
        /// <summary>
        /// ウィンドウサービス
        /// </summary>
        private readonly WindowService windowService;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="windowService">ウィンドウサービス</param>
        public ControlCapturer(WindowService windowService)
        {
            this.windowService = windowService;
        }

        /// <summary>
        /// キャプチャのコア処理。
        /// </summary>
        /// <param name="target">キャプチャ対象</param>
        /// <returns>キャプチャ画像</returns>
        protected override Bitmap CaptureCore(IntPtr? target)
        {
            return ScreenHelper.CaptureScreen(target.Value);
        }

        /// <summary>
        /// キャプチャ対象を取得します。
        /// </summary>
        /// <returns>キャプチャ対象</returns>
        protected override IntPtr? GetTargetCore()
        {
            var handle = this.windowService.ShowControlSelectionWindow();
            return (handle != IntPtr.Zero
                ? handle as IntPtr?
                : null);
        }
    }
}
