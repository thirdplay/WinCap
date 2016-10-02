namespace WinCap.Util.Serialization
{
    /// <summary>
    /// シリアル化プロパティを表すクラス。
    /// </summary>
    /// <typeparam name="T">任意のクラス</typeparam>
    public sealed class SerializableProperty<T> : SerializablePropertyBase<T>
    {
        public SerializableProperty(string key, ISerializationProvider provider) : base(key, provider) { }

        public SerializableProperty(string key, ISerializationProvider provider, T defaultValue) : base(key, provider, defaultValue) { }
    }
}
