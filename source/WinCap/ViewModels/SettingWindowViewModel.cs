using Livet;
using System.Collections.Generic;
using System.Linq;
using WinCap.Models;
using WinCap.Utility.Mvvm;
using WinCap.ViewModels.Settings;

namespace WinCap.ViewModels
{
    /// <summary>
    /// 設定ウィンドウViewModel
    /// </summary>
    public class SettingWindowViewModel : ViewModel
    {
        /// <summary>
        /// タイトル
        /// </summary>
        public string Title => ProductInfo.Title;

        #region TanItems ViewModel
        /// <summary>
        /// 基本設定ViewModel
        /// </summary>
        public BasicViewModel Basic { get; }
        #endregion

        /// <summary>
        /// タブ項目
        /// </summary>
        public IList<TabItemViewModel> TabItems { get; set; }

        #region SelectedItem 変更通知プロパティ
        private TabItemViewModel _SelectedItem;
        /// <summary>
        /// 選択項目を取得、設定します。
        /// </summary>
        public TabItemViewModel SelectedItem
        {
            get { return this._SelectedItem; }
            set
            {
                if (this._SelectedItem != value)
                {
                    this._SelectedItem = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SettingWindowViewModel()
        {
            this.TabItems = new List<TabItemViewModel>
            {
                (this.Basic = new BasicViewModel().AddTo(this))
            };
            this.SelectedItem = this.TabItems.FirstOrDefault();
        }

        /// <summary>
		/// <see cref="Window.ContentRendered"/>イベントが発生したときに
        /// Livet インフラストラクチャによって呼び出されます。
        /// </summary>
        public void Initialize()
        {
        }
    }
}
