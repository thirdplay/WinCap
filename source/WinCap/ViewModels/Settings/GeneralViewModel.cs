using Livet;
using WinCap.Util.Mvvm;

namespace WinCap.ViewModels.Settings
{
    /// <summary>
    /// 一般設定のためのデータを提供します。
    /// </summary>
    public class GeneralViewModel : ViewModel
    {
        #region IsRegisterInStartup 変更通知プロパティ
        private bool _IsRegisterInStartup;
        /// <summary>
        /// スタートアップに登録する
        /// </summary>
        public bool IsRegisterInStartup
        {
            get { return _IsRegisterInStartup; }
            set
            {
                if (_IsRegisterInStartup != value)
                {
                    this._IsRegisterInStartup = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region IsPlaySeWhenCapture 変更通知プロパティ
        private bool _IsPlaySeWhenCapture;
        /// <summary>
        /// キャプチャー時に効果音を再生する
        /// </summary>
        public bool IsPlaySeWhenCapture
        {
            get { return _IsPlaySeWhenCapture; }
            set
            {
                if (_IsPlaySeWhenCapture != value)
                {
                    this._IsPlaySeWhenCapture = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region WebPageCaptureStartWhenPageFirstMove 変更通知プロパティ
        private bool _WebPageCaptureStartWhenPageFirstMove;
        /// <summary>
        /// ウェブページ全体キャプチャ開始時にページ先頭に移動する
        /// </summary>
        public bool WebPageCaptureStartWhenPageFirstMove
        {
            get { return _WebPageCaptureStartWhenPageFirstMove; }
            set
            {
                if (_WebPageCaptureStartWhenPageFirstMove != value)
                {
                    this._WebPageCaptureStartWhenPageFirstMove = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            this.RevertToSavedSettings();
        }

        /// <summary>
        /// 適用
        /// </summary>
        public void Apply()
        {
            var settings = Serialization.Settings.General;
            settings.IsRegisterInStartup.Value = this.IsRegisterInStartup;
            settings.IsPlaySeWhenCapture.Value = this.IsPlaySeWhenCapture;
            settings.WebPageCaptureStartWhenPageFirstMove.Value = this.WebPageCaptureStartWhenPageFirstMove;
        }

        /// <summary>
        /// キャンセル
        /// </summary>
        public void Cancel()
        {
            this.RevertToSavedSettings();
        }

        /// <summary>
        /// 保存時の設定に戻します。
        /// </summary>
        private void RevertToSavedSettings()
        {
            var settings = Serialization.Settings.General;
            this.IsRegisterInStartup = settings.IsRegisterInStartup;
            this.IsPlaySeWhenCapture = settings.IsPlaySeWhenCapture;
            this.WebPageCaptureStartWhenPageFirstMove = settings.WebPageCaptureStartWhenPageFirstMove;
        }
    }
}