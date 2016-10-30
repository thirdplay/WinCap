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
        /// 全てのプロパティの入力値を検証する
        /// </summary>
        /// <returns>検証エラーがある場合はtrue、それ以外はfalse</returns>
        bool ValidateAll();

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
