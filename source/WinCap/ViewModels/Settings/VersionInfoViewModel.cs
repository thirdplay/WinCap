﻿using Livet;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using WinCap.Properties;

namespace WinCap.ViewModels.Settings
{
    /// <summary>
    /// バージョン情報のためのデータを提供します。
    /// </summary>
    public class VersionInfoViewModel : SettingsBaseViewModel
    {
        /// <summary>
        /// ライブラリコンテナ
        /// </summary>
        public IReadOnlyCollection<BindableTextViewModel> Libraries { get; }

        #region TabItemViewModel mebmers

        /// <summary>
        /// タブ名を取得します。
        /// </summary>
        public override string Name
        {
            get { return Resources.Settings_VersionInfo; }
            protected set { throw new NotImplementedException(); }
        }

        #endregion TabItemViewModel mebmers

        public VersionInfoViewModel()
        {
            this.Libraries = ProductInfo.Libraries.Aggregate(
                new List<BindableTextViewModel>(),
                (list, lib) =>
                {
                    list.Add(new BindableTextViewModel { Text = list.Count == 0 ? "Build with " : ", " });
                    list.Add(new HyperlinkViewModel { Text = lib.Name.Replace(' ', Convert.ToChar(160)), Uri = lib.Url });
                    return list;
                });
        }

        #region WindowViewModel members

        /// <summary>
        /// <see cref="System.Windows.Window.ContentRendered"/> イベントが発生したときに呼び出される初期化処理。
        /// </summary>
        protected override void InitializeCore()
        {
            base.InitializeCore();
        }

        #endregion WindowViewModel members

        #region ISettingsBaseViewModel members

        /// <summary>
        /// 入力値を検証する
        /// </summary>
        /// <returns>検証結果</returns>
        public override bool Validate()
        {
            return true;
        }

        /// <summary>
        /// 適用
        /// </summary>
        public override void Apply()
        {
        }

        /// <summary>
        /// キャンセル
        /// </summary>
        public override void Cancel()
        {
        }

        #endregion ISettingsBaseViewModel members
    }

    /// <summary>
    /// バインド可能なテキストを提供します。
    /// </summary>
    public class BindableTextViewModel : ViewModel
    {
        #region Text 変更通知プロパティ

        private string _Text;

        /// <summary>
        /// テキストを取得します。
        /// </summary>
        public string Text
        {
            get { return this._Text; }
            set
            {
                if (this._Text != value)
                {
                    this._Text = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion Text 変更通知プロパティ
    }

    /// <summary>
    /// ハイパーリンクを提供します。
    /// </summary>
    public class HyperlinkViewModel : BindableTextViewModel
    {
        #region Uri 変更通知プロパティ

        private Uri _Uri;

        /// <summary>
        /// URIを取得します。
        /// </summary>
        public Uri Uri
        {
            get { return this._Uri; }
            set
            {
                if (this._Uri != value)
                {
                    this._Uri = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion Uri 変更通知プロパティ
    }
}