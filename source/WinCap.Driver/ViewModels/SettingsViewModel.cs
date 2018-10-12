using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows.Grasp;

namespace WinCap.Driver.ViewModels
{
    /// <summary>
    /// 設定ウィンドウのViewModelを操作する機能を提供します。
    /// </summary>
    public class SettingsViewModel
    {
        /// <summary>
        /// 全般タブのViewModel
        /// </summary>
        public GeneralViewModel General { get; private set; }

        /// <summary>
        /// 出力タブのViewModel
        /// </summary>
        public OutputViewModel Output { get; private set; }

        /// <summary>
        /// ショートカットキータブのViewModel
        /// </summary>
        public ShortcutKeyViewModel ShortcutKey { get; private set; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public SettingsViewModel(WindowControl windowControl)
        {
            var viewModel = windowControl.Dynamic().DataContext;
            General = new GeneralViewModel(viewModel.General);
            Output = new OutputViewModel(viewModel.Output);
            ShortcutKey = new ShortcutKeyViewModel(viewModel.ShortcutKey);
        }
    }
}
