﻿namespace WinCap.Serialization
{
    /// <summary>
    /// アプリケーションの設定
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// 一般設定
        /// </summary>
        public static GeneralSettings General => new GeneralSettings(LocalSettingsProvider.Instance);

        /// <summary>
        /// 出力設定
        /// </summary>
        public static OutputSettings Output => new OutputSettings(LocalSettingsProvider.Instance);

        /// <summary>
        /// ショートカットキー設定
        /// </summary>
        public static ShortcutKeySettings ShortcutKey => new ShortcutKeySettings(LocalSettingsProvider.Instance);
    }
}
