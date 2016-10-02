using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WinCap.Services;

namespace WinCap.Serialization
{
    /// <summary>
    /// シリアル化の拡張機能を提供します。
    /// </summary>
    public static class SerializationExtensions
    {
        /// <summary>
        /// ショートカットキーのキャッシュ
        /// </summary>
        private static readonly Dictionary<string, ShortcutKey> _keyCache = new Dictionary<string, ShortcutKey>();

        /// <summary>
        /// ショートカットキーキャッシュをクリアします。
        /// </summary>
        public static void ClearCache() => _keyCache.Clear();

        /// <summary>
        /// ショートカットキープロパティをショートカットキーに変換します。
        /// </summary>
        /// <param name="property">ショートカットキープロパティ</param>
        /// <returns>ショートカットキー</returns>
        public static ShortcutKey ToShortcutKey(this ShortcutkeyProperty property)
        {
            if (property?.Value == null) { return ShortcutKey.None; }

            ShortcutKey cached;
            if (_keyCache.TryGetValue(property.Key, out cached))
            {
                return cached;
            }

            var result = ToShortcutKey(property.Value);
            _keyCache[property.Key] = result;

            return result;
        }

        /// <summary>
        /// 仮想キー配列をショートカットキーに変換します。
        /// </summary>
        /// <param name="keyCodes">仮想キー配列</param>
        /// <returns>ショートカットキー</returns>
        public static ShortcutKey ToShortcutKey(this int[] keyCodes)
        {
            if (keyCodes == null) { return ShortcutKey.None; }

            var key = keyCodes.Length >= 1 ? (Keys)keyCodes[0] : Keys.None;
            var modifiers = keyCodes.Length >= 2 ? keyCodes.Skip(1).Select(x => (Keys)x).ToArray() : Array.Empty<Keys>();
            var result = new ShortcutKey(key, modifiers);

            return result;
        }

        public static int[] ToSerializable(this ShortcutKey shortcutKey)
        {
            if (shortcutKey.Key == Keys.None) return Array.Empty<int>();

            var key = new[] { (int)shortcutKey.Key, };

            return shortcutKey.Modifiers.Length == 0
                ? key
                : key.Concat(shortcutKey.Modifiers.Select(x => (int)x)).ToArray();
        }
    }
}
