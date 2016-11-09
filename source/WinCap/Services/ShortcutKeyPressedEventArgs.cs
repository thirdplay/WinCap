using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Input;

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
        /// <param name="modifierKeys">装飾キーセット</param>
        internal ShortcutKeyPressedEventArgs(Key key, ModifierKeys modifierKeys)
        {
            this.ShortcutKey = new ShortcutKey(key, modifierKeys);
        }
    }
}
