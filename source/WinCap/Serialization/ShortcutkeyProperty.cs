using System;
using System.Windows.Input;
using WinCap.Services;
using WinCap.Util.Serialization;

namespace WinCap.Serialization
{
    /// <summary>
    /// ショートカットキープロパティ
    /// </summary>
    public class ShortcutkeyProperty : SerializablePropertyBase<ShortcutKey>
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="provider">シリアライズ化機能の提供者</param>
        public ShortcutkeyProperty(string key, ISerializationProvider provider) : base(key, provider) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="provider">シリアライズ化機能の提供者</param>
        /// <param name="defaultValue">デフォルト値</param>
        public ShortcutkeyProperty(string key, ISerializationProvider provider, ShortcutKey defaultValue) : base(key, provider, defaultValue) { }

        /// <summary>
        /// シリアライズのコア処理。
        /// </summary>
        /// <param name="value">シリアライズ前の値</param>
        /// <returns>シリアライズ後の値</returns>
        protected override object SerializeCore(ShortcutKey value)
        {
            if (value == ShortcutKey.None) { return string.Empty; }

            return value.Key.ToString() + "," + value.ModifierKeys.ToString();
        }

        /// <summary>
        /// デシリアライズのコア処理。
        /// </summary>
        /// <param name="value">デシリアライズ前の値</param>
        /// <returns>デシリアライズ後の値</returns>
        protected override ShortcutKey DeserializeCore(object value)
        {
            var data = value as string;
            if (data == null) { return base.DeserializeCore(value); }

            if (string.IsNullOrEmpty(data)) { return ShortcutKey.None; }
            if (data.Split(',').Length < 2) { return ShortcutKey.None; }

            return new ShortcutKey(
                (Key)Enum.Parse(typeof(Key), data.Split(',')[0]),
                (ModifierKeys)Enum.Parse(typeof(ModifierKeys), data.Split(',')[1]));
        }
    }
}
