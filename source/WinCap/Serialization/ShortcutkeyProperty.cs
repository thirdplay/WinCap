using System;
using System.Globalization;
using System.Linq;
using WpfUtility.Linq;
using WpfUtility.Serialization;

namespace WinCap.Serialization
{
    /// <summary>
    /// ショートカットキープロパティ
    /// </summary>
    public class ShortcutkeyProperty : SerializablePropertyBase<int[]>
    {
        /// <summary>
        /// 空データ文字列
        /// </summary>
        private const string empryString = "(none)";

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
        public ShortcutkeyProperty(string key, ISerializationProvider provider, int[] defaultValue) : base(key, provider, defaultValue) { }

        /// <summary>
        /// シリアライズのコア処理。
        /// </summary>
        /// <param name="value">シリアライズ前の値</param>
        /// <returns>シリアライズ後の値</returns>
        protected override object SerializeCore(int[] value)
        {
            if (value == null || value.Length == 0) { return empryString; }

            return value
                .Select(x => x.ToString(CultureInfo.InvariantCulture))
                .JoinString(",");
        }

        /// <summary>
        /// デシリアライズのコア処理。
        /// </summary>
        /// <param name="value">デシリアライズ前の値</param>
        /// <returns>デシリアライズ後の値</returns>
        protected override int[] DeserializeCore(object value)
        {
            var data = value as string;
            if (data == null) { return base.DeserializeCore(value); }

            if (string.IsNullOrEmpty(data)) { return null; }
            if (string.Equals(data, empryString, StringComparison.OrdinalIgnoreCase)) { return Array.Empty<int>(); }

            return data.Split(',')
                .Select(x => int.Parse(x))
                .ToArray();
        }
    }
}
