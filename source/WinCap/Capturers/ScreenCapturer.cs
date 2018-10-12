using System.Drawing;

namespace WinCap.Capturers
{
    /// <summary>
    /// 画面をキャプチャする機能を提供します。
    /// </summary>
    public class ScreenCapturer : CapturerBase<Rectangle?>
    {
        #region CapturerBase members

        /// <summary>
        /// キャプチャ対象を取得します。
        /// </summary>
        /// <returns>キャプチャ対象</returns>
        protected override Rectangle? GetCaptureTarget()
        {
            return ScreenHelper.GetFullScreenBounds();
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
