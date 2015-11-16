using MetroTrilithon.Serialization;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace WinCap.Models.Settings
{
    /// <summary>
    /// ホットキーに関連する設定を表す静的プロパティを公開します。
    /// </summary>
    public class HotkeySettings
    {
        /// <summary>
        /// 画面全体キャプチャ時のホットキー
        /// </summary>
        public static SerializableProperty<Keys> ScreenWholeKey { get; }
            = new SerializableProperty<Keys>(GetKey(), Providers.Local, Keys.PrintScreen);

        /// <summary>
        /// アクティブウィンドウキャプチャ時のホットキー
        /// </summary>
        public static SerializableProperty<Keys> ActiveWindowKey { get; }
            = new SerializableProperty<Keys>(GetKey(), Providers.Local, Keys.Alt | Keys.PrintScreen);

        /// <summary>
        /// 選択コントロールキャプチャ時のホットキー
        /// </summary>
        public static SerializableProperty<Keys> SelectControlKey { get; }
            = new SerializableProperty<Keys>(GetKey(), Providers.Local, Keys.Control | Keys.PrintScreen);

        /// <summary>
        /// ページ全体キャプチャ時のホットキー
        /// </summary>
        public static SerializableProperty<Keys> PageWholeKey { get; }
            = new SerializableProperty<Keys>(GetKey(), Providers.Local, Keys.Alt | Keys.Control | Keys.PrintScreen);

        private static string GetKey([CallerMemberName] string propertyName = "")
        {
            return nameof(HotkeySettings) + "." + propertyName;
        }
    }
}
