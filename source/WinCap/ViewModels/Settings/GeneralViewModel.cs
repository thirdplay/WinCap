﻿using System;
using System.ComponentModel.DataAnnotations;
using WinCap.Properties;

namespace WinCap.ViewModels.Settings
{
    /// <summary>
    /// 一般設定のためのデータを提供します。
    /// </summary>
    public class GeneralViewModel : TabItemViewModel, ISettingsViewModel
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
        private int _ScrollDelayTime;
        /// <summary>
        /// スクロール時の遅延時間を取得します。
        /// </summary>
        [Range(0, 1000, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validation_Range")]
        public int ScrollDelayTime
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
        private int _CaptureDelayTime;
        /// <summary>
        /// キャプチャ時の遅延時間を取得します。
        /// </summary>
        [Range(0, 10000, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "Validation_Range")]
        public int CaptureDelayTime
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

        #region ISettingsViewModel members
        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            this.RevertToSavedSettings();
        }

        /// <summary>
        /// 入力値を検証する
        /// </summary>
        /// <returns>検証結果</returns>
        public bool Validate()
        {
            return base.ValidateAll();
        }

        /// <summary>
        /// 適用
        /// </summary>
        public void Apply()
        {
            var settings = Serialization.Settings.General;
            settings.IsRegisterInStartup.Value = this.IsRegisterInStartup;
            settings.IsPlaySeWhenCapture.Value = this.IsPlaySeWhenCapture;
            settings.IsWebPageCaptureStartWhenPageFirstMove.Value = this.IsWebPageCaptureStartWhenPageFirstMove;
            settings.ScrollDelayTime.Value = this.ScrollDelayTime;
            settings.CaptureDelayTime.Value = this.CaptureDelayTime;
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
            this.IsWebPageCaptureStartWhenPageFirstMove = settings.IsWebPageCaptureStartWhenPageFirstMove;
            this.ScrollDelayTime = settings.ScrollDelayTime;
            this.CaptureDelayTime = settings.CaptureDelayTime;
        }
        #endregion
    }
}