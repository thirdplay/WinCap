using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Input;
using WinCap.Interop;

namespace WinCap.Services
{
    /// <summary>
    /// ショートカットキーを表します。([modifer key(s)] + [key] style)
    /// </summary>
    public struct ShortcutKey
    {
        /// <summary>
        /// キーコード
        /// </summary>
        public Key Key { get; }

        /// <summary>
        /// 修飾キーセット
        /// </summary>
        public ModifierKeys ModifierKeys { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="key">キーコード</param>
        public ShortcutKey(Key key)
            : this(key, ModifierKeys.None)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="key">キーコード</param>
        /// <param name="modifiers">修飾キーセット</param>
        public ShortcutKey(Key key, ModifierKeys modifierKeys)
        {
            this.Key = key;
            this.ModifierKeys = modifierKeys;
        }

        /// <summary>
        /// ショートカットキーの比較。
        /// </summary>
        /// <param name="other">別のショートカットキー</param>
        /// <returns>等価の場合はtrue、それ以外はfalse</returns>
        public bool Equals(ShortcutKey other)
        {
            return this == other;
        }

        /// <summary>
        /// Equalsのオーバーライド。
        /// </summary>
        /// <param name="obj">比較対象</param>
        /// <returns>等価の場合はtrue、それ以外はfalse</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            return obj is ShortcutKey && this.Equals((ShortcutKey)obj);
        }

        /// <summary>
        /// ハッシュコードを取得します。
        /// </summary>
        /// <returns>ハッシュコード</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.ModifierKeys.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)this.Key;
                return hashCode;
            }
        }

        /// <summary>
        /// 文字列に変換します。
        /// </summary>
        /// <returns>文字列</returns>
        public override string ToString()
        {
            if (this == ShortcutKey.None) { return ""; }

            var sb = new StringBuilder();
            if ((this.ModifierKeys & ModifierKeys.Alt) == ModifierKeys.Alt)
            {
                sb.Append(getLocalizedKeyName((int)VK.MENU));
                sb.Append(" + ");
            }
            if ((this.ModifierKeys & ModifierKeys.Control) == ModifierKeys.Control)
            {
                sb.Append(getLocalizedKeyName((int)VK.CONTROL));
                sb.Append(" + ");
            }
            if ((this.ModifierKeys & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                sb.Append(getLocalizedKeyName((int)VK.SHIFT));
                sb.Append(" + ");
            }
            if ((this.ModifierKeys & ModifierKeys.Windows) == ModifierKeys.Windows)
            {
                sb.Append("Windows + ");
            }

            if (this.Key == Key.PrintScreen)
            {
                // 識別しやすくするために、固有名称を設定する
                sb.Append("Print Screen");
            }
            else
            {
                sb.Append(getLocalizedKeyName(KeyInterop.VirtualKeyFromKey(this.Key)));
            }

            return sb.ToString();
        }

        /// <summary>
        /// キー名称を取得します。
        /// </summary>
        /// <param name="key">仮想キーコード</param>
        /// <returns>キー名称</returns>
        private static string getLocalizedKeyName(int key)
        {
            // 修飾キーを削除
            long keyCode = key & 0xffff;

            var sb = new StringBuilder(256);

            long scanCode = NativeMethods.MapVirtualKey((uint)keyCode, (int)MAPVK.VK_TO_VSC);

            // 上位ワードにスキャンコードをシフトする
            scanCode = (scanCode << 16);
            if (keyCode == 45 ||
                keyCode == 46 ||
                keyCode == 144 ||
                (33 <= keyCode && keyCode <= 40))
            {
                // 拡張キーフラグ追加
                scanCode |= 0x1000000;
            }

            if (NativeMethods.GetKeyNameText((int)scanCode, sb, 256) == 0)
            {
                return "";
            }
            return sb.ToString();
        }

        /// <summary>
        /// 等価演算子のオーバーライド。
        /// </summary>
        /// <param name="key1">比較対象1</param>
        /// <param name="key2">比較対象2</param>
        /// <returns>等価ならtrue、それ以外はfalse</returns>
        public static bool operator ==(ShortcutKey key1, ShortcutKey key2)
        {
            return key1.Key == key2.Key && key1.ModifierKeys == key2.ModifierKeys;
        }

        /// <summary>
        /// 非等価演算子のオーバーライド。
        /// </summary>
        /// <param name="key1">比較対象1</param>
        /// <param name="key2">比較対象2</param>
        /// <returns>非等価ならtrue、それ以外はfalse</returns>
        public static bool operator !=(ShortcutKey key1, ShortcutKey key2)
        {
            return !(key1 == key2);
        }

        /// <summary>
        /// ショートカットキーの比較。
        /// </summary>
        /// <param name="key1">比較対象1</param>
        /// <param name="key2">比較対象2</param>
        /// <returns>等価ならtrue、それ以外はfalse</returns>
        private static bool Equals(ICollection<Keys> key1, ICollection<Keys> key2)
        {
            return key1.Count == key2.Count && !key1.Except(key2).Any();
        }

        /// <summary>
        /// ショートカットキーなしを表す
        /// </summary>
        public static readonly ShortcutKey None = new ShortcutKey(Key.None);
    }
}
