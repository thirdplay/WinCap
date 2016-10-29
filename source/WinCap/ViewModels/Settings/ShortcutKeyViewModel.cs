using Livet;
using WinCap.Serialization;
using WinCap.Services;

namespace WinCap.ViewModels.Settings
{
    /// <summary>
    /// ショートカットキー設定のためのデータを提供します。
    /// </summary>
    public class ShortcutKeyViewModel : ViewModel
    {
        #region FullScreen 変更通知プロパティ
        private int[] _FullScreen;
        public int[] FullScreen
        {
            get { return _FullScreen; }
            set
            { 
                if (_FullScreen != value)
                {
                    _FullScreen = value;
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
            var settings = Serialization.Settings.ShortcutKey;
            settings.FullScreen.Value = this.FullScreen;
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
            var settings = Serialization.Settings.ShortcutKey;
            this.FullScreen = settings.FullScreen;
        }
    }
}
