using Livet.Messaging.IO;
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
        #endregion

        #region OutputMethodType 変更通知プロパティ
        private OutputMethodType _OutputMethodType;
        public OutputMethodType OutputMethodType
        {
            get { return _OutputMethodType; }
            set
            {
                if (_OutputMethodType != value)
                {
                    _OutputMethodType = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region OutputFolder 変更通知プロパティ
        private string _OutputFolder;
        public string OutputFolder
        {
            get { return _OutputFolder; }
            set
            {
                if (_OutputFolder != value)
                {
                    _OutputFolder = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region IsAutoSaveImage 変更通知プロパティ
        private bool _IsAutoSaveImage;
        public bool IsAutoSaveImage
        {
            get { return _IsAutoSaveImage; }
            set
            { 
                if (_IsAutoSaveImage != value)
                {
                    _IsAutoSaveImage = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region OutputFileNamePattern 変更通知プロパティ
        private string _OutputFileNamePattern;
        public string OutputFileNamePattern
        {
            get { return _OutputFileNamePattern; }
            set
            { 
                if (_OutputFileNamePattern != value)
                {
                    _OutputFileNamePattern = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region OutputFormatType 変更通知プロパティ
        private OutputFormatType _OutputFormatType;
        public OutputFormatType OutputFormatType
        {
            get { return _OutputFormatType; }
            set
            { 
                if (_OutputFormatType != value)
                {
                    _OutputFormatType = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region SettingsBaseViewModel members
        /// <summary>
        /// 初期化
        /// </summary>
        public override void Initialize()
        {
            this.RevertToSavedSettings();
        }

        /// <summary>
        /// 入力値を検証する
        /// </summary>
        /// <returns>検証結果</returns>
        public override bool Validate()
        {
            return this.ValidateAll();
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

        /// <summary>
        /// 出力先選択ダイアログを開きます。
        /// </summary>
        public void OpenOutputFolderSelectionDialog()
        {
            var message = new FolderSelectionMessage("FolderDialog.Open")
            {
                Title = "test",//Resources.Settings_Screenshot_FolderSelectionDialog_Title,
                DialogPreference = Helper.IsWindows8OrGreater
                    ? FolderSelectionDialogPreference.CommonItemDialog
                    : FolderSelectionDialogPreference.FolderBrowser,
                //SelectedPath = this.CanOpenDestination ? ScreenshotSettings.Destination : ""
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
        #endregion
    }
}
