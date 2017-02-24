using MetroRadiance.UI.Controls;
using WinCap.ViewModels.Settings;
using WpfUtility.Mvvm;

namespace WinCap.ViewModels
{
    /// <summary>
    /// タブ項目のためのデータを提供する基底クラスです。
    /// </summary>
    public abstract class TabItemViewModel : ValidatableViewModel, ITabItem, ISettingsBaseViewModel
    {
        #region Badge 変更通知プロパティ
        private int? _Badge;
        /// <summary>
        /// バッジを取得します。
        /// </summary>
        public virtual int? Badge
        {
            get { return this._Badge; }
            protected set
            {
                if (this._Badge != value)
                {
                    this._Badge = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region IsSelected 変更通知プロパティ
        private bool _IsSelected;
        /// <summary>
        /// 選択状態を取得します。
        /// </summary>
        public virtual bool IsSelected
        {
            get { return this._IsSelected; }
            set
            {
                if (this._IsSelected != value)
                {
                    this._IsSelected = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region Name 変更通知プロパティ
        /// <summary>
        /// タブ名を取得します。
        /// </summary>
        public abstract string Name { get; protected set; }
        #endregion

        /// <summary>
        /// 入力値を検証する
        /// </summary>
        /// <returns>検証結果</returns>
        public abstract bool Validate();

        /// <summary>
        /// 適用
        /// </summary>
        public abstract void Apply();

        /// <summary>
        /// キャンセル
        /// </summary>
        public abstract void Cancel();
    }
}
