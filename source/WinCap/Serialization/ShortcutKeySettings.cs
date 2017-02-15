﻿using System.Windows.Input;
using WpfUtility.Serialization;

namespace WinCap.Serialization
{
    /// <summary>
    /// ショートカットキー設定のアクセスを提供します。
    /// </summary>
    public class ShortcutKeySettings : SettingsHost
    {
        /// <summary>
        /// シリアル化機能
        /// </summary>
        private readonly ISerializationProvider provider;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="provider">シリアル化機能の提供者</param>
        public ShortcutKeySettings(ISerializationProvider provider)
        {
            this.provider = provider;
        }

        /// <summary>
        /// 画面全体をキャプチャ
        /// </summary>
        public ShortcutkeyProperty FullScreen => this.Cache(key => new ShortcutkeyProperty(key, this.provider, FullScreenDefaultValue));

        /// <summary>
        /// アクティブコントロールをキャプチャ
        /// </summary>
        public ShortcutkeyProperty ActiveControl => this.Cache(key => new ShortcutkeyProperty(key, this.provider, ActiveControlDefaultValue));

        /// <summary>
        /// 選択コントロールをキャプチャ
        /// </summary>
        public ShortcutkeyProperty SelectionControl => this.Cache(key => new ShortcutkeyProperty(key, this.provider, SelectControlDefaultValue));

        /// <summary>
        /// ウェブページ全体をキャプチャ
        /// </summary>
        public ShortcutkeyProperty WebPage => this.Cache(key => new ShortcutkeyProperty(key, this.provider, WebPageDefaultValue));

        #region default values
        private static int[] FullScreenDefaultValue { get; } = {
            (int)Key.PrintScreen
        };
        private static int[] ActiveControlDefaultValue { get; } = {
            (int)Key.PrintScreen,
            (int)ModifierKeys.Alt
        };
        private static int[] SelectControlDefaultValue { get; } = {
            (int)Key.PrintScreen,
            (int)ModifierKeys.Control
        };
        private static int[] WebPageDefaultValue { get; } = {
            (int)Key.PrintScreen,
            (int)(ModifierKeys.Control | ModifierKeys.Alt)
        };
        #endregion
    }
}
