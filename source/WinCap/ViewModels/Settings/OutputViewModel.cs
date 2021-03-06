﻿using Livet.Messaging.IO;
using System;
using System.IO;
using WinCap.Models;
using WinCap.Properties;

namespace WinCap.ViewModels.Settings
{
    /// <summary>
    /// 出力設定のためのデータを提供します。
    /// </summary>
    public class OutputViewModel : SettingsBaseViewModel
    {
        #region TabItemViewModel mebmers

        /// <summary>
        /// タブ名を取得します。
        /// </summary>
        public override string Name
        {
            get { return Resources.Settings_Output; }
            protected set { throw new NotImplementedException(); }
        }

        #endregion TabItemViewModel mebmers

        #region OutputMethodType 変更通知プロパティ

        private OutputMethodType _OutputMethodType;

        /// <summary>
        /// 出力方法を取得します。
        /// </summary>
        public OutputMethodType OutputMethodType
        {
            get { return this._OutputMethodType; }
            set
            {
                if (this._OutputMethodType != value)
                {
                    this._OutputMethodType = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion OutputMethodType 変更通知プロパティ

        #region OutputFolder 変更通知プロパティ

        private string _OutputFolder;

        /// <summary>
        /// 出力フォルダを取得します。
        /// </summary>
        public string OutputFolder
        {
            get { return this._OutputFolder; }
            set
            {
                if (this._OutputFolder != value)
                {
                    this._OutputFolder = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion OutputFolder 変更通知プロパティ

        #region IsAutoSaveImage 変更通知プロパティ

        private bool _IsAutoSaveImage;

        /// <summary>
        /// 画像を自動保存するか取得します。
        /// </summary>
        public bool IsAutoSaveImage
        {
            get { return this._IsAutoSaveImage; }
            set
            {
                if (this._IsAutoSaveImage != value)
                {
                    this._IsAutoSaveImage = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion IsAutoSaveImage 変更通知プロパティ

        #region OutputFileNamePattern 変更通知プロパティ

        private string _OutputFileNamePattern;

        /// <summary>
        /// 出力ファイル名パターンを取得します。
        /// </summary>
        public string OutputFileNamePattern
        {
            get { return this._OutputFileNamePattern; }
            set
            {
                if (this._OutputFileNamePattern != value)
                {
                    this._OutputFileNamePattern = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion OutputFileNamePattern 変更通知プロパティ

        #region OutputFormatType 変更通知プロパティ

        private OutputFormatType _OutputFormatType;

        /// <summary>
        /// 出力形式を取得します。
        /// </summary>
        public OutputFormatType OutputFormatType
        {
            get { return this._OutputFormatType; }
            set
            {
                if (this._OutputFormatType != value)
                {
                    this._OutputFormatType = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion OutputFormatType 変更通知プロパティ

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
            this.ClearErrors();
            if (this.IsAutoSaveImage && string.IsNullOrEmpty(this.OutputFolder))
            {
                // 画像を自動保存する場合、出力フォルダは必須
                this.SetError(nameof(this.OutputFolder), Resources.Validation_Required);
                return false;
            }
            if (!string.IsNullOrEmpty(this.OutputFolder) && !Directory.Exists(this.OutputFolder))
            {
                // 入力した出力フォルダが存在しない場合はNG
                this.SetError(nameof(this.OutputFolder), Resources.Settings_NotFoundOutputFolderMessage);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 適用
        /// </summary>
        public override void Apply()
        {
            var settings = Serialization.Settings.Output;
            settings.OutputMethodType.Value = this.OutputMethodType;
            settings.OutputFolder.Value = this.OutputFolder;
            settings.IsAutoSaveImage.Value = this.IsAutoSaveImage;
            settings.OutputFileNamePattern.Value = this.OutputFileNamePattern;
            settings.OutputFormatType.Value = this.OutputFormatType;
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
        /// 出力先選択ダイアログを開きます。
        /// </summary>
        public void OpenOutputFolderSelectionDialog()
        {
            var message = new FolderSelectionMessage("FolderDialog.Open")
            {
                Title = Resources.Settings_OutputFolderSelectionDialog_Title,
                DialogPreference = Helper.IsWindows8OrGreater
                    ? FolderSelectionDialogPreference.CommonItemDialog
                    : FolderSelectionDialogPreference.FolderBrowser,
                SelectedPath = Directory.Exists(this.OutputFolder) ? this.OutputFolder : ""
            };
            this.Messenger.Raise(message);

            if (Directory.Exists(message.Response))
            {
                this.OutputFolder = message.Response;
            }
        }

        /// <summary>
        /// 保存時の設定に戻します。
        /// </summary>
        private void RevertToSavedSettings()
        {
            var settings = Serialization.Settings.Output;
            this.OutputMethodType = settings.OutputMethodType;
            this.OutputFolder = settings.OutputFolder;
            this.IsAutoSaveImage = settings.IsAutoSaveImage;
            this.OutputFileNamePattern = settings.OutputFileNamePattern;
            this.OutputFormatType = settings.OutputFormatType;
        }
    }
}