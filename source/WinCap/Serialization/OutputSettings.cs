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
        public EnumProperty<OutputMethodType> OutputMethodType => this.Cache(key => new EnumProperty<OutputMethodType>(key, this._provider, Models.OutputMethodType.Clipboard));

        /// <summary>
        /// 出力フォルダ
        /// </summary>
        public SerializableProperty<string> OutputFolder => this.Cache(key => new SerializableProperty<string>(key, this._provider));

        /// <summary>
        /// 画像を自動保存する
        /// </summary>
        public SerializableProperty<bool> IsAutoSaveImage => this.Cache(key => new SerializableProperty<bool>(key, this._provider));

        /// <summary>
        /// 出力ファイル名パターン
        /// </summary>
        public SerializableProperty<string> OutputFileNamePattern => this.Cache(key => new SerializableProperty<string>(key, this._provider, "Image %c"));

        /// <summary>
        /// 出力形式
        /// </summary>
        public EnumProperty<OutputFormatType> OutputFormatType => this.Cache(key => new EnumProperty<OutputFormatType>(key, this._provider, Models.OutputFormatType.Png));
    }
}
