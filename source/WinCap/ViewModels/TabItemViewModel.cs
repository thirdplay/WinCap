using MetroRadiance.Controls;

namespace WinCap.ViewModels
{
    /// <summary>
    /// タブ項目のためのデータを提供します。このクラスは抽象クラスです。
    /// </summary>
    public abstract class TabItemViewModel : ItemViewModel, ITabItem
    {
        #region Name 変更通知プロパティ
        /// <summary>
        /// タブ名を取得します。
        /// </summary>
        public abstract string Name { get; protected set; }
        #endregion

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
                    RaisePropertyChanged();
                }
            }
        }
        #endregion
    }
}
