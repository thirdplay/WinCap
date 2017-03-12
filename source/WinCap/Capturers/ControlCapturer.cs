using System;
using System.Drawing;
using WinCap.Interop;
using WinCap.Interop.Win32;
using WinCap.ViewModels;
using WinCap.Views;

namespace WinCap.Capturers
{
    /// <summary>
    /// コントロールをキャプチャーする機能を提供します。
    /// </summary>
    public class ControlCapturer : CapturerBase<IntPtr?>
    {
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
            var viewModel = new ControlSelectionWindowViewModel();
            var window = new ControlSelectionWindow { DataContext = viewModel };
            viewModel.Initialized = () => window.Activate();
            window.ShowDialog();
            window.Close();

            return (viewModel.SelectedHandle != IntPtr.Zero
                ? viewModel.SelectedHandle as IntPtr?
                : null);
        }
    }
}
