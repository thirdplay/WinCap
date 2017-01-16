using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using WinCap.Services;

namespace WinCap.Serialization
{
    /// <summary>
    /// シリアル化の拡張機能を提供します。
    /// </summary>
    public static class SerializationExtensions
    {
        /// <summary>
        /// 仮想キー配列をショートカットキーに変換します。
        /// </summary>
        /// <param name="keyCodes">仮想キー配列</param>
        /// <returns>ショートカットキー</returns>
        public static ShortcutKey ToShortcutKey(this int[] keyCodes)
        {
            if (keyCodes == null) { return ShortcutKey.None; }

            var key = keyCodes.Length >= 1 ? (Key)keyCodes[0] : Key.None;
            var modifierKeys = keyCodes.Length >= 2 ? (ModifierKeys)keyCodes[1] : ModifierKeys.None;
            var result = new ShortcutKey(key, modifierKeys);

            return result;
        }

        /// <summary>
        /// ショートカットキーを仮想キー配列に変換します。
        /// </summary>
        /// <param name="shortcutKey">ショートカットキー</param>
        /// <returns>仮想キー配列</returns>
        public static int[] ToSerializable(this ShortcutKey shortcutKey)
        {
            if (shortcutKey.Key == Key.None) { return Array.Empty<int>(); }

            var key = new[] { (int)shortcutKey.Key, };

            return shortcutKey.ModifierKeys == ModifierKeys.None
                ? key
                : key.Concat(new int[] { (int)shortcutKey.ModifierKeys }).ToArray();
        }
    }
}
