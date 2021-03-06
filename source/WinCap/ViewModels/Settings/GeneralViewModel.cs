﻿using System;
using System.ComponentModel.DataAnnotations;
using WinCap.Properties;

namespace WinCap.ViewModels.Settings
{
    /// <summary>
    /// 一般設定のためのデータを提供します。
    /// </summary>
    public class GeneralViewModel : SettingsBaseViewModel
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

        #endregion TabItemViewModel mebmers

        #region IsRegisterInStartup 変更通知プロパティ

        private bool _IsRegisterInStartup;

        /// <summary>
        /// スタートアップに登録するか取得します。
        /// </summary>
        public bool IsRegisterInStartup
        {
            get { return this._IsRegisterInStartup; }
            set
            {
                if (this._IsRegisterInStartup != value)
                {
                    this._IsRegisterInStartup = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion IsRegisterInStartup 変更通知プロパティ

        #region IsCreateShortcutToDesktop 変更通知プロパティ

        private bool _IsCreateShortcutToDesktop;

        /// <summary>
        /// デスクトップにショートカットを作成するか取得します。
        /// </summary>
        public bool IsCreateShortcutToDesktop
        {
            get { return this._IsCreateShortcutToDesktop; }
            set
            {
                if (this._IsCreateShortcutToDesktop != value)
                {
                    this._IsCreateShortcutToDesktop = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion IsCreateShortcutToDesktop 変更通知プロパティ

        #region IsPlaySeWhenCapture 変更通知プロパティ

        private bool _IsPlaySeWhenCapture;

        /// <summary>
        /// キャプチャー時に効果音を再生するか取得します。
        /// </summary>
        public bool IsPlaySeWhenCapture
        {
            get { return this._IsPlaySeWhenCapture; }
            set
            {
                if (this._IsPlaySeWhenCapture != value)
                {
                    this._IsPlaySeWhenCapture = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion IsPlaySeWhenCapture 変更通知プロパティ

        #region IsWebPageCaptureStartWhenPageFirstMove 変更通知プロパティ

        private bool _IsWebPageCaptureStartWhenPageFirstMove;

        /// <summary>
        /// ウェブページ全体キャプチャ開始時にページ先頭に移動するか取得します。
        /// </summary>
        public bool IsWebPageCaptureStartWhenPageFirstMove
        {
            get { return this._IsWebPageCaptureStartWhenPageFirstMove; }
            set
            {
                if (this._IsWebPageCaptureStartWhenPageFirstMove != value)
                {
                    this._IsWebPageCaptureStartWhenPageFirstMove = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion IsWebPageCaptureStartWhenPageFirstMove 変更通知プロパティ

        #region ScrollDelayTime 変更通知プロパティ

        private string _ScrollDelayTime;

        /// <summary>
        /// スクロール時の遅延時間を取得します。
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validation_Required")]
        [Range(0, 1000, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validation_Range")]
        public string ScrollDelayTime
        {
            get { return this._ScrollDelayTime; }
            set
            {
                if (this._ScrollDelayTime != value)
                {
                    this._ScrollDelayTime = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion ScrollDelayTime 変更通知プロパティ

        #region CaptureDelayTime 変更通知プロパティ

        private string _CaptureDelayTime;

        /// <summary>
        /// キャプチャ時の遅延時間を取得します。
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validation_Required")]
        [Range(0, 10000, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validation_Range")]
        public string CaptureDelayTime
        {
            get { return this._CaptureDelayTime; }
            set
            {
                if (this._CaptureDelayTime != value)
                {
                    this._CaptureDelayTime = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion CaptureDelayTime 変更通知プロパティ

        #region FixHeaderHeight 変更通知プロパティ

        private string _FixHeaderHeight;

        /// <summary>
        /// 固定ヘッダーの高さを取得します。
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validation_Required")]
        [Range(0, 10000, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validation_Range")]
        public string FixHeaderHeight
        {
            get { return this._FixHeaderHeight; }
            set
            {
                if (this._FixHeaderHeight != value)
                {
                    this._FixHeaderHeight = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion FixHeaderHeight 変更通知プロパティ

        #region WindowViewModel members

        /// <summary>
        /// <see cref="System.Windows.Window.ContentRendered"/> イベントが発生したときに呼び出される初期化処理。
        /// </summary>
        protected override void InitializeCore()
        {
            this.RevertToSavedSettings();
        }

        #endregion WindowViewModel members

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
            settings.FixHeaderHeight.Value = int.Parse(this.FixHeaderHeight);
        }

        /// <summary>
        /// キャンセル
        /// </summary>
        public override void Cancel()
        {
            this.RevertToSavedSettings();
        }

        #endregion TabItemViewModel members

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
            this.FixHeaderHeight = settings.FixHeaderHeight.Value.ToString();
        }
    }
}