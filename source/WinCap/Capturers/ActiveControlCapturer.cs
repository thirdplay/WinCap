using System.Drawing;
using WinCap.Interop;
using WinCap.Interop.Win32;

namespace WinCap.Capturers
{
    /// <summary>
    /// アクティブコントロールをキャプチャする機能を提供します。
    /// </summary>
    public class ActiveControlCapturer : CapturerBase<Rectangle?>
    {
        /// <summary>
        /// キャプチャのコア処理。
        /// </summary>
        /// <param name="target">キャプチャ対象</param>
        /// <returns>キャプチャ画像</returns>
        protected override Bitmap CaptureCore(Rectangle? target)
        {
            return ScreenHelper.CaptureScreen(target.Value);
        }

        /// <summary>
        /// キャプチャ対象を取得します。
        /// </summary>
        /// <returns>キャプチャ対象</returns>
        protected override Rectangle? GetTargetCore()
        {
            return InteropHelper.GetWindowSize(User32.GetForegroundWindow(), false);
        }
    }
}
