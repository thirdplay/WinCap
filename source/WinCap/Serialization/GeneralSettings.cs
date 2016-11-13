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
        private readonly ISerializationProvider provider;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="provider">シリアル化機能提供者</param>
        public GeneralSettings(ISerializationProvider provider)
        {
            this.provider = provider;
        }

        /// <summary>
        /// スタートアップに登録する
        /// </summary>
        public SerializableProperty<bool> IsRegisterInStartup => this.Cache(key => new SerializableProperty<bool>(key, this.provider, true));

        /// <summary>
        /// キャプチャー時に効果音を再生する
        /// </summary>
        public SerializableProperty<bool> IsPlaySeWhenCapture => this.Cache(key => new SerializableProperty<bool>(key, this.provider, true));

        /// <summary>
        /// ウェブページ全体キャプチャ開始時にページ先頭に移動する
        /// </summary>
        public SerializableProperty<bool> IsWebPageCaptureStartWhenPageFirstMove => this.Cache(key => new SerializableProperty<bool>(key, this.provider, true));

        /// <summary>
        /// スクロール遅延時間
        /// </summary>
        public SerializableProperty<int> ScrollDelayTime => this.Cache(key => new SerializableProperty<int>(key, this.provider, 100));

        /// <summary>
        /// キャプチャ遅延時間
        /// </summary>
        public SerializableProperty<int> CaptureDelayTime => this.Cache(key => new SerializableProperty<int>(key, this.provider, 0));
    }
}
