using System;
using System.Drawing;
using WinCap.Services;

namespace WinCap.Capturers
{
    /// <summary>
    /// 範囲をキャプチャーする機能を提供します。
    /// </summary>
    public class RegionCapturer : CapturerBase<Rectangle?>
    {
        /// <summary>
        /// ウィンドウサービス
        /// </summary>
        private readonly WindowService windowService;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="windowService">ウィンドウサービス</param>
        public RegionCapturer(WindowService windowService)
        {
            this.windowService = windowService;
        }

        #region CapturerBase members

        /// <summary>
        /// キャプチャ対象を取得します。
        /// </summary>
        /// <returns>キャプチャ対象</returns>
        protected override Rectangle? GetCaptureTarget()
        {
            return this.windowService.ShowRegionSelectionWindow();
        }

        /// <summary>
        /// キャプチャのコア処理。
        /// </summary>
        /// <param name="target">キャプチャ対象</param>
        /// <returns>キャプチャ画像</returns>
        protected override Bitmap CaptureCore(Rectangle? target)
        {
            return ScreenHelper.CaptureScreen(target.Value);
        }

        #endregion
    }
}
