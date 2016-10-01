using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WinCap.Util.Linq;

namespace WinCap.Services
{
    /// <summary>
    /// ショートカットキーを表します。([modifer key(s)] + [key] style)
    /// </summary>
    public struct ShortcutKey
    {
        public Keys Key { get; }
        public Keys[] Modifiers { get; }

        internal ICollection<Keys> ModifiersInternal { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="key">仮想キー</param>
        /// <param name="modifiers">装飾キー配列</param>
        public ShortcutKey(Keys key, params Keys[] modifiers)
        {
            this.Key = key;
            this.Modifiers = modifiers;
            this.ModifiersInternal = modifiers;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="key">仮想キー</param>
        /// <param name="modifiers">装飾キーリスト</param>
        internal ShortcutKey(Keys key, ICollection<Keys> modifiers) : this()
        {
            this.Key = key;
            this.ModifiersInternal = modifiers;
        }

        /// <summary>
        /// ショートカットキーの比較
        /// </summary>
        /// <param name="other">別のショートカットキー</param>
        /// <returns></returns>
        public bool Equals(ShortcutKey other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
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
                var hashCode = (this.ModifiersInternal ?? this.Modifiers)?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (int)this.Key;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return (this.ModifiersInternal ?? this.Modifiers ?? Enumerable.Empty<Keys>())
                .OrderBy(x => x)
                .Select(x => x + " + ")
                .Concat(EnumerableEx.Return(this.Key == Keys.None ? "" : this.Key.ToString()))
                .JoinString("");
        }

        public static bool operator ==(ShortcutKey key1, ShortcutKey key2)
        {
            return key1.Key == key2.Key
                   && Equals(
                       key1.ModifiersInternal ?? key1.Modifiers ?? Array.Empty<Keys>(),
                       key2.ModifiersInternal ?? key2.Modifiers ?? Array.Empty<Keys>());
        }

        public static bool operator !=(ShortcutKey key1, ShortcutKey key2)
        {
            return !(key1 == key2);
        }

        private static bool Equals(ICollection<Keys> key1, ICollection<Keys> key2)
        {
            return key1.Count == key2.Count && !key1.Except(key2).Any();
        }


        public static readonly ShortcutKey None = new ShortcutKey(Keys.None);
    }
}
