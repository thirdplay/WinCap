using Codeer.Friendly;
using Codeer.Friendly.Dynamic;

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
        public GeneralViewModel General { get; set; }

        /// <summary>
        /// 出力タブのViewModel
        /// </summary>
        public OutputViewModel Output { get; set; }

        /// <summary>
        /// ショートカットキータブのViewModel
        /// </summary>
        public ShortcutKeyViewModel ShortcutKey { get; set; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public SettingsViewModel()
        {
        }
    }
}
