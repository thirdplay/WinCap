namespace WinCap.ViewModels.Settings
{
    /// <summary>
    /// 設定ウィンドウのタブ項目の振る舞いを提供する仮想クラス。
    /// </summary>
    public abstract class SettingsBaseViewModel : TabItemViewModel
    {
        /// <summary>
        /// 入力値を検証する
        /// </summary>
        /// <returns>検証結果</returns>
        public abstract bool Validate();

        /// <summary>
        /// 適用
        /// </summary>
        public abstract void Apply();

        /// <summary>
        /// キャンセル
        /// </summary>
        public abstract void Cancel();
    }
}