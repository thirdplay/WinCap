using Codeer.Friendly;

namespace WinCap.Driver.ViewModels
{
    /// <summary>
    /// 設定ウィンドウの出力タブのViewModelを操作する機能を提供します。
    /// </summary>
    public class OutputViewModel : ViewModelBase
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="appVar">アプリケーション内変数</param>
        public OutputViewModel(AppVar appVar)
            : base(appVar)
        {
        }
    }
}
