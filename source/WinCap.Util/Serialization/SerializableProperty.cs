namespace WinCap.Util.Serialization
{
    /// <summary>
    /// シリアル化プロパティを表すクラス。
    /// </summary>
    /// <typeparam name="T">任意のクラス</typeparam>
    public sealed class SerializableProperty<T> : SerializablePropertyBase<T>
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="provider">シリアライズ化機能の提供者</param>
        public SerializableProperty(string key, ISerializationProvider provider) : base(key, provider) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="provider">シリアライズ化機能の提供者</param>
        /// <param name="defaultValue">デフォルト値</param>
        public SerializableProperty(string key, ISerializationProvider provider, T defaultValue) : base(key, provider, defaultValue) { }
    }
}
