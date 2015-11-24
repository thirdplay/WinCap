using Livet;

namespace WinCap.ViewModels
{
    /// <summary>
    /// 項目のためのデータを提供します。
    /// </summary>
    public class ItemViewModel : ViewModel
    {
        #region IsSelected 変更通知プロパティ
        private bool _IsSelected;

        /// <summary>
        /// 選択状態を取得します。
        /// </summary>
        public bool IsSelected
        {
            get { return this._IsSelected; }
            set {
                if (this._IsSelected != value)
                {
                    this._IsSelected = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
