using WinCap.Models;
using WinCap.Util.Serialization;

namespace WinCap.Serialization
{
    /// <summary>
    /// 出力設定のアクセスを提供します。
    /// </summary>
    public class OutputSettings : SettingsHost
    {
        /// <summary>
        /// シリアル化機能
        /// </summary>
        private readonly ISerializationProvider _provider;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="provider">シリアル化機能提供者</param>
        public OutputSettings(ISerializationProvider provider)
        {
            this._provider = provider;
        }

        /// <summary>
        /// 出力方法
        /// </summary>
        public SerializableProperty<OutputMethodType> OutputMethodType => this.Cache(key => new SerializableProperty<OutputMethodType>(key, this._provider, Models.OutputMethodType.Clipboard));
    }
}
