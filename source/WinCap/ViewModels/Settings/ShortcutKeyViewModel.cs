﻿using System;
using System.Collections.Generic;
using System.Linq;
using WinCap.Properties;
using WinCap.Serialization;
using WinCap.ShortcutKeys;

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

        #endregion TabItemViewModel mebmers

        #region FullScreen 変更通知プロパティ

        private int[] _FullScreen;

        /// <summary>
        /// 画面全体をキャプチャするショートカットキーを取得します。
        /// </summary>
        public int[] FullScreen
        {
            get { return this._FullScreen; }
            set
            {
                if (this._FullScreen != value)
                {
                    this._FullScreen = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion FullScreen 変更通知プロパティ

        #region ActiveControl 変更通知プロパティ

        private int[] _ActiveControl;

        /// <summary>
        /// アクティブコントロールをキャプチャするショートカットキーを取得します。
        /// </summary>
        public int[] ActiveControl
        {
            get { return this._ActiveControl; }
            set
            {
                if (this._ActiveControl != value)
                {
                    this._ActiveControl = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion ActiveControl 変更通知プロパティ

        #region SelectionControl 変更通知プロパティ

        private int[] _SelectionControl;

        /// <summary>
        /// 選択コントロールをキャプチャするショートカットキーを取得します。
        /// </summary>
        public int[] SelectionControl
        {
            get { return this._SelectionControl; }
            set
            {
                if (this._SelectionControl != value)
                {
                    this._SelectionControl = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion SelectionControl 変更通知プロパティ

        #region SelectionRegion 変更通知プロパティ

        private int[] _SelectionRegion;

        /// <summary>
        /// 選択領域をキャプチャするショートカットキーを取得します。
        /// </summary>
        public int[] SelectionRegion
        {
            get { return this._SelectionRegion; }
            set
            {
                if (this._SelectionRegion != value)
                {
                    this._SelectionRegion = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion SelectionRegion 変更通知プロパティ

        #region WebPage 変更通知プロパティ

        private int[] _WebPage;

        /// <summary>
        /// Webページ全体をキャプチャするショートカットキーを取得します。
        /// </summary>
        public int[] WebPage
        {
            get { return this._WebPage; }
            set
            {
                if (this._WebPage != value)
                {
                    this._WebPage = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion WebPage 変更通知プロパティ

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
            var shortcutKeies = new Dictionary<string, ShortcutKey>
            {
                {nameof(this.FullScreen), this.FullScreen.ToShortcutKey()},
                {nameof(this.ActiveControl),  this.ActiveControl.ToShortcutKey()},
                {nameof(this.SelectionControl), this.SelectionControl.ToShortcutKey()},
                {nameof(this.SelectionRegion), this.SelectionRegion.ToShortcutKey()},
                {nameof(this.WebPage), this.WebPage.ToShortcutKey()},
            };

            // 重複するショートカットキーがあればエラー
            this.ClearErrors();
            foreach (var pair in shortcutKeies)
            {
                if (pair.Value == ShortcutKey.None) { continue; }
                if (shortcutKeies.Values.Where(x => x != ShortcutKey.None && x == pair.Value).Count() > 1)
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
            settings.SelectionRegion.Value = this.SelectionRegion;
            settings.WebPage.Value = this.WebPage;
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
            var settings = Serialization.Settings.ShortcutKey;
            this.FullScreen = settings.FullScreen;
            this.ActiveControl = settings.ActiveControl;
            this.SelectionControl = settings.SelectionControl;
            this.SelectionRegion = settings.SelectionRegion;
            this.WebPage = settings.WebPage;
        }
    }
}