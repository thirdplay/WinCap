using System;
using WinCap.Properties;

namespace WinCap.ViewModels.Settings
{
    /// <summary>
    /// ショートカットキー設定のためのデータを提供します。
    /// </summary>
    public class ShortcutKeyViewModel : SettingsBaseViewModel
    {
        #region TabItemViewModel mebmers
        /// <summary>
        /// タブ名を取得します。
        /// </summary>
        public override string Name
        {
            get { return Resources.Settings_ShortcutKey; }
            protected set { throw new NotImplementedException(); }
        }
        #endregion

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
            var settings = Serialization.Settings.ShortcutKey;
            settings.FullScreen.Value = this.FullScreen;
        }

        /// <summary>
        /// キャンセル
        /// </summary>
        public override void Cancel()
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
        #endregion
    }
}
