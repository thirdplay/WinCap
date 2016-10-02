namespace WinCap.Serialization
{
    /// <summary>
    /// アプリケーションの設定
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// 一般設定
        /// </summary>
        public static GeneralSettings General { get; } = new GeneralSettings(LocalSettingsProvider.Instance);

        /// <summary>
        /// ショートカット設定
        /// </summary>
        public static ShortcutKeySettings ShortcutKey { get; } = new ShortcutKeySettings(LocalSettingsProvider.Instance);
    }
}
