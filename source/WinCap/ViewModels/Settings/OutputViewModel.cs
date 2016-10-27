using Livet;
using WinCap.Models;

namespace WinCap.ViewModels.Settings
{
    /// <summary>
    /// 出力設定のためのデータを提供します。
    /// </summary>
    public class OutputViewModel : ViewModel
    {
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
        public void Cancel()
        {
            this.RevertToSavedSettings();
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
