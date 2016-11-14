using Livet;

namespace WinCap.ViewModels.Settings
{
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
        #endregion
    }
}
