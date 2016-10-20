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
        /// <summary>
        /// 出力方法
        /// </summary>
        public OutputMethodType OutputMethodType
        {
            get { return _OutputMethodType; }
            set
            {
                if (_OutputMethodType != value)
                {
                    this._OutputMethodType = value;
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
            var settings = Serialization.Settings.Output;
            settings.OutputMethodType.Value = this.OutputMethodType;
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
        }
    }
}
