using WinCap.Util.Serialization;
using Keys = System.Windows.Forms.Keys;

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
        private readonly ISerializationProvider _provider;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="provider"></param>
        public ShortcutKeySettings(ISerializationProvider provider)
        {
            this._provider = provider;
        }

        /// <summary>
        /// 画面全体をキャプチャ
        /// </summary>
        public ShortcutkeyProperty FullScreen => (ShortcutkeyProperty)this.Cache(key => new ShortcutkeyProperty(key, this._provider, FullScreenDefaultValue));

        /// <summary>
        /// アクティブコントロールをキャプチャ
        /// </summary>
        public ShortcutkeyProperty ActiveControl => (ShortcutkeyProperty)this.Cache(key => new ShortcutkeyProperty(key, this._provider, ActiveControlDefaultValue));

        /// <summary>
        /// 選択コントロールをキャプチャ
        /// </summary>
        public ShortcutkeyProperty SelectControl => (ShortcutkeyProperty)this.Cache(key => new ShortcutkeyProperty(key, this._provider, SelectControlDefaultValue));

        #region default values
        private static int[] FullScreenDefaultValue { get; } = {
            (int)Keys.PrintScreen
        };

        private static int[] ActiveControlDefaultValue { get; } = {
            (int)Keys.PrintScreen,
            (int)Keys.Alt
        };

        private static int[] SelectControlDefaultValue { get; } = {
            (int)Keys.PrintScreen,
            (int)Keys.LControlKey
        };
        #endregion
    }
}
