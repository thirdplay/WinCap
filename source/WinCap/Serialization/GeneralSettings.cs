using WinCap.Util.Serialization;

namespace WinCap.Serialization
{
    /// <summary>
    /// 一般設定のアクセスを提供します。
    /// </summary>
    public class GeneralSettings : SettingsHost
    {
        /// <summary>
        /// シリアル化機能
        /// </summary>
        private readonly ISerializationProvider _provider;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="provider"></param>
        public GeneralSettings(ISerializationProvider provider)
        {
            this._provider = provider;
        }

        public SerializableProperty<bool> TestProp1 => this.Cache(key => new SerializableProperty<bool>(key, this._provider));
        public SerializableProperty<bool> TestProp2 => this.Cache(key => new SerializableProperty<bool>(key, this._provider));
        public SerializableProperty<bool> TestProp3 => this.Cache(key => new SerializableProperty<bool>(key, this._provider));
    }
}
