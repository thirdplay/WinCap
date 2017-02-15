using WinCap.Models;
using WpfUtility.Serialization;

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
        private readonly ISerializationProvider provider;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="provider">シリアル化機能提供者</param>
        public OutputSettings(ISerializationProvider provider)
        {
            this.provider = provider;
        }

        /// <summary>
        /// 出力方法
        /// </summary>
        public EnumProperty<OutputMethodType> OutputMethodType => this.Cache(key => new EnumProperty<OutputMethodType>(key, this.provider, Models.OutputMethodType.Clipboard));

        /// <summary>
        /// 出力フォルダ
        /// </summary>
        public SerializableProperty<string> OutputFolder => this.Cache(key => new SerializableProperty<string>(key, this.provider));

        /// <summary>
        /// 画像を自動保存する
        /// </summary>
        public SerializableProperty<bool> IsAutoSaveImage => this.Cache(key => new SerializableProperty<bool>(key, this.provider));

        /// <summary>
        /// 出力ファイル名パターン
        /// </summary>
        public SerializableProperty<string> OutputFileNamePattern => this.Cache(key => new SerializableProperty<string>(key, this.provider, "Image %c"));

        /// <summary>
        /// 出力形式
        /// </summary>
        public EnumProperty<OutputFormatType> OutputFormatType => this.Cache(key => new EnumProperty<OutputFormatType>(key, this.provider, Models.OutputFormatType.Png));
    }
}
