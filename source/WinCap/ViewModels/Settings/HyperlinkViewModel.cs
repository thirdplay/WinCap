using System;

namespace WinCap.ViewModels.Settings
{
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
        #endregion
    }
}
