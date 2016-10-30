namespace WinCap.ViewModels.Settings
{
    /// <summary>
    /// 設定の機能を提供します。
    /// </summary>
    public interface ISettingsViewModel
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
