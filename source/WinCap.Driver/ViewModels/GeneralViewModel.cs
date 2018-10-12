using Codeer.Friendly;

namespace WinCap.Driver.ViewModels
{
    /// <summary>
    /// 設定ウィンドウの全般タブのViewModelを操作する機能を提供します。
    /// </summary>
    public class GeneralViewModel : ViewModelBase
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="appVar">アプリケーション内変数</param>
        public GeneralViewModel(AppVar appVar)
            : base(appVar)
        {
        }
    }
}
