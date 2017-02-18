namespace WinCap.ViewModels.Settings
{
    /// <summary>
    /// 設定ウィンドウのタブ項目の振る舞いを提供するインターフェイス。
    /// </summary>
    public interface ISettingsBaseViewModel
    {
        /// <summary>
        /// 初期化
        /// </summary>
        void Initialize();

        /// <summary>
        /// 入力値を検証する
        /// </summary>
        /// <returns>検証結果</returns>
        bool Validate();

        /// <summary>
        /// 適用
        /// </summary>
        void Apply();

        /// <summary>
        /// キャンセル
        /// </summary>
        void Cancel();
    }
}
