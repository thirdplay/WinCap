using Codeer.Friendly;

namespace WinCap.Driver.ViewModels
{
    /// <summary>
    /// 設定ウィンドウのショートカットキータブのViewModelを操作する機能を提供します。
    /// </summary>
    public class ShortcutKeyViewModel : ViewModelBase
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="appVar">アプリケーション内変数</param>
        public ShortcutKeyViewModel(AppVar appVar)
            : base(appVar)
        {
        }
    }
}
