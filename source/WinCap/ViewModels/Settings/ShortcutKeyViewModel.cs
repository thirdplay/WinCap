using Livet.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using WinCap.Properties;
using WinCap.Services;

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
        private ShortcutKey _FullScreen;
        /// <summary>
        /// 画面全体をキャプチャするショートカットキーを取得します。
        /// </summary>
        public ShortcutKey FullScreen
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

        #region ActiveControl 変更通知プロパティ
        private ShortcutKey _ActiveControl;
        /// <summary>
        /// アクティブコントロールをキャプチャするショートカットキーを取得します。
        /// </summary>
        public ShortcutKey ActiveControl
        {
            get { return _ActiveControl; }
            set
            {
                if (_ActiveControl != value)
                {
                    _ActiveControl = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region SelectionControl 変更通知プロパティ
        private ShortcutKey _SelectionControl;
        /// <summary>
        /// 選択コントロールをキャプチャするショートカットキーを取得します。
        /// </summary>
        public ShortcutKey SelectionControl
        {
            get { return _SelectionControl; }
            set
            {
                if (_SelectionControl != value)
                {
                    _SelectionControl = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region WebPage 変更通知プロパティ
        private ShortcutKey _WebPage;
        /// <summary>
        /// Webページ全体をキャプチャするショートカットキーを取得します。
        /// </summary>
        public ShortcutKey WebPage
        {
            get { return _WebPage; }
            set
            {
                if (_WebPage != value)
                {
                    _WebPage = value;
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
            this.revertToSavedSettings();
        }

        /// <summary>
        /// 入力値を検証する
        /// </summary>
        /// <returns>検証結果</returns>
        public override bool Validate()
        {
            var shortcutKeys = new Dictionary<string, ShortcutKey>
            {
                {nameof(this.FullScreen), this.FullScreen},
                {nameof(this.ActiveControl),  this.ActiveControl},
                {nameof(this.SelectionControl), this.SelectionControl},
                {nameof(this.WebPage), this.WebPage},
            };

            // 重複するショートカットキーがあればエラー
            this.ClearErrors();
            foreach (var pair in shortcutKeys)
            {
                if (pair.Value == ShortcutKey.None) { continue; }
                if (shortcutKeys.Values.Where(x => x != ShortcutKey.None && x == pair.Value).Count() > 1)
                {
                    this.SetError(pair.Key, Resources.Settings_ShortcutKeyDuplicationMessage);
                }
            }

            return !this.HasErrors;
        }

        /// <summary>
        /// 適用
        /// </summary>
        public override void Apply()
        {
            var settings = Serialization.Settings.ShortcutKey;
            settings.FullScreen.Value = this.FullScreen;
            settings.ActiveControl.Value = this.ActiveControl;
            settings.SelectionControl.Value = this.SelectionControl;
            settings.WebPage.Value = this.WebPage;
        }

        /// <summary>
        /// キャンセル
        /// </summary>
        public override void Cancel()
        {
            this.revertToSavedSettings();
        }

        /// <summary>
        /// 保存時の設定に戻します。
        /// </summary>
        private void revertToSavedSettings()
        {
            var settings = Serialization.Settings.ShortcutKey;
            this.FullScreen = settings.FullScreen;
            this.ActiveControl = settings.ActiveControl;
            this.SelectionControl = settings.SelectionControl;
            this.WebPage = settings.WebPage;
        }
        #endregion
    }
}
