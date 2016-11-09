using System;

namespace WinCap.Util.Serialization
{
    /// <summary>
    /// 列挙型プロパティ
    /// </summary>
    /// <typeparam name="T">列挙型</typeparam>
    public class EnumProperty<T> : SerializablePropertyBase<T> where T : struct
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="provider">シリアライズ化機能の提供者</param>
        /// <param name="defaultValue">デフォルト値</param>
        public EnumProperty(string key, ISerializationProvider provider, T defaultValue) : base(key, provider, defaultValue) { }

        /// <summary>
        /// シリアライズのコア処理。
        /// </summary>
        /// <param name="value">シリアライズ前の値</param>
        /// <returns>シリアライズ後の値</returns>
        protected override object SerializeCore(T value)
        {
            return value.ToString();
        }

        /// <summary>
        /// デシリアライズのコア処理。
        /// </summary>
        /// <param name="value">デシリアライズ前の値</param>
        /// <returns>デシリアライズ後の値</returns>
        protected override T DeserializeCore(object value)
        {
            var data = value as string;
            if (data == null) { return base.DeserializeCore(value); }
            if (string.IsNullOrEmpty(data)) { return this.Default; }

            T result;
            if (!Enum.TryParse(data, out result)) { return this.Default; }

            return result;
        }
    }
}
