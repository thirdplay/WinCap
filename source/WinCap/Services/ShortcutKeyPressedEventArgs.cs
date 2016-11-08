using System.Collections.Generic;
using System.Windows.Forms;

namespace WinCap.Services
{
    /// <summary>
    /// ショートカットキー押下イベントの引数クラスです。
    /// </summary>
    public class ShortcutKeyPressedEventArgs
    {
        /// <summary>
        /// 押下したショートカットキー
        /// </summary>
        public ShortcutKey ShortcutKey { get; }

        /// <summary>
        /// 繰り返し押されたキーかどうか
        /// </summary>
        public bool IsRepeat { get; }

        /// <summary>
        /// イベントの処理状態
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="shortcutKey">ショートカットキー</param>
        public ShortcutKeyPressedEventArgs(ShortcutKey shortcutKey)
        {
            this.ShortcutKey = shortcutKey;
        }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="key">キーコード</param>
        /// <param name="modifiers">装飾キーコード</param>
        /// <param name="isRepeat">繰り返し押されたキーかどうか</param>
        internal ShortcutKeyPressedEventArgs(Keys key, ICollection<Keys> modifiers, bool isRepeat)
        {
            this.ShortcutKey = new ShortcutKey(key, modifiers);
            this.IsRepeat = isRepeat;
        }
    }
}
