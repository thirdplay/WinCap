using System;
using System.ComponentModel.DataAnnotations;
using WinCap.Properties;

namespace WinCap.ViewModels.Settings
{
    /// <summary>
    /// 一般設定のためのデータを提供します。
    /// </summary>
    public class GeneralViewModel : TabItemViewModel
    {
        #region TabItemViewModel mebmers
        /// <summary>
        /// タブ名を取得します。
        /// </summary>
        public override string Name
        {
            get { return Resources.Settings_General; }
            protected set { throw new NotImplementedException(); }
        }
        #endregion

        #region IsRegisterInStartup 変更通知プロパティ
        private bool _IsRegisterInStartup;
        /// <summary>
        /// スタートアップに登録するか取得します。
        /// </summary>
        public bool IsRegisterInStartup
        {
            get { return _IsRegisterInStartup; }
            set
            {
                if (_IsRegisterInStartup != value)
                {
                    _IsRegisterInStartup = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region IsCreateShortcutToDesktop 変更通知プロパティ
        private bool _IsCreateShortcutToDesktop;
        /// <summary>
        /// デスクトップにショートカットを作成するか取得します。
        /// </summary>
        public bool IsCreateShortcutToDesktop
        {
            get { return _IsCreateShortcutToDesktop; }
            set
            { 
                if (_IsCreateShortcutToDesktop != value)
                {
                    _IsCreateShortcutToDesktop = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region IsPlaySeWhenCapture 変更通知プロパティ
        private bool _IsPlaySeWhenCapture;
        /// <summary>
        /// キャプチャー時に効果音を再生するか取得します。
        /// </summary>
        public bool IsPlaySeWhenCapture
        {
            get { return _IsPlaySeWhenCapture; }
            set
            {
                if (_IsPlaySeWhenCapture != value)
                {
                    _IsPlaySeWhenCapture = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region IsWebPageCaptureStartWhenPageFirstMove 変更通知プロパティ
        private bool _IsWebPageCaptureStartWhenPageFirstMove;
        /// <summary>
        /// ウェブページ全体キャプチャ開始時にページ先頭に移動するか取得します。
        /// </summary>
        public bool IsWebPageCaptureStartWhenPageFirstMove
        {
            get { return _IsWebPageCaptureStartWhenPageFirstMove; }
            set
            {
                if (_IsWebPageCaptureStartWhenPageFirstMove != value)
                {
                    _IsWebPageCaptureStartWhenPageFirstMove = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region ScrollDelayTime 変更通知プロパティ
        private string _ScrollDelayTime;
        /// <summary>
        /// スクロール時の遅延時間を取得します。
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validation_Required")]
        [Range(0, 1000, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validation_Range")]
        public string ScrollDelayTime
        {
            get { return _ScrollDelayTime; }
            set
            {
                if (_ScrollDelayTime != value)
                {
                    _ScrollDelayTime = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region CaptureDelayTime 変更通知プロパティ
        private string _CaptureDelayTime;
        /// <summary>
        /// キャプチャ時の遅延時間を取得します。
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validation_Required")]
        [Range(0, 10000, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validation_Range")]
        public string CaptureDelayTime
        {
            get { return _CaptureDelayTime; }
            set
            {
                if (_CaptureDelayTime != value)
                {
                    _CaptureDelayTime = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region WindowViewModel members

        /// <summary>
        /// <see cref="System.Windows.Window.ContentRendered"/> イベントが発生したときに呼び出される初期化処理。
        /// </summary>
        protected override void InitializeCore()
        {
            this.RevertToSavedSettings();
        }

        #endregion

        #region TabItemViewModel members

        /// <summary>
        /// 入力値を検証する
        /// </summary>
        /// <returns>検証結果</returns>
        public override bool Validate()
        {
            return base.ValidateAll();
        }

        /// <summary>
        /// 適用
        /// </summary>
        public override void Apply()
        {
            var settings = Serialization.Settings.General;
            settings.IsRegisterInStartup.Value = this.IsRegisterInStartup;
            settings.IsCreateShortcutToDesktop.Value = this.IsCreateShortcutToDesktop;
            settings.IsPlaySeWhenCapture.Value = this.IsPlaySeWhenCapture;
            settings.IsWebPageCaptureStartWhenPageFirstMove.Value = this.IsWebPageCaptureStartWhenPageFirstMove;
            settings.ScrollDelayTime.Value = int.Parse(this.ScrollDelayTime);
            settings.CaptureDelayTime.Value = int.Parse(this.CaptureDelayTime);
        }

        /// <summary>
        /// キャンセル
        /// </summary>
        public override void Cancel()
        {
            this.RevertToSavedSettings();
        }

        #endregion

        /// <summary>
        /// 保存時の設定に戻します。
        /// </summary>
        private void RevertToSavedSettings()
        {
            var settings = Serialization.Settings.General;
            this.IsRegisterInStartup = settings.IsRegisterInStartup;
            this.IsCreateShortcutToDesktop = settings.IsCreateShortcutToDesktop;
            this.IsPlaySeWhenCapture = settings.IsPlaySeWhenCapture;
            this.IsWebPageCaptureStartWhenPageFirstMove = settings.IsWebPageCaptureStartWhenPageFirstMove;
            this.ScrollDelayTime = settings.ScrollDelayTime.Value.ToString();
            this.CaptureDelayTime = settings.CaptureDelayTime.Value.ToString();
        }
    }
}