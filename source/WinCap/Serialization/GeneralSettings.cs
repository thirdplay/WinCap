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

        /// <summary>
        /// スタートアップに登録する
        /// </summary>
        public SerializableProperty<bool> IsRegisterInStartup => this.Cache(key => new SerializableProperty<bool>(key, this._provider, false));

        /// <summary>
        /// キャプチャー時に効果音を再生する
        /// </summary>
        public SerializableProperty<bool> IsPlaySeWhenCapture => this.Cache(key => new SerializableProperty<bool>(key, this._provider, true));

        /// <summary>
        /// ウェブページ全体キャプチャ開始時にページ先頭に移動する
        /// </summary>
        public SerializableProperty<bool> WebPageCaptureStartWhenPageFirstMove => this.Cache(key => new SerializableProperty<bool>(key, this._provider, true));
    }
}
