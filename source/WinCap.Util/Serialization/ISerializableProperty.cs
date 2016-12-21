namespace WinCap.Util.Serialization
{
    /// <summary>
    /// シリアル化可能なプロパティのインターフェイス。
    /// </summary>
    public interface ISerializableProperty
    {
        /// <summary>
        /// シリアル化機能の提供者を取得します。
        /// </summary>
        ISerializationProvider Provider { get; }

        /// <summary>
        /// プロパティを表すキーを取得します。
        /// </summary>
        string Key { get; }

        /// <summary>
        /// 自動保存状態を取得または設定します。
        /// </summary>
        bool AutoSave { get; set; }

        /// <summary>
        /// プロパティをリセットします。
        /// </summary>
        void Reset();
    }
}
