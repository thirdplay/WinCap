using Livet;
using System.Drawing;

namespace WinCap.ViewModels
{
    /// <summary>
    /// コントロール選択情報ViewModel
    /// </summary>
    public class ControlSelectInfoViewModel : ViewModel
    {
        #region ClassName 変更通知プロパティ
        private string _ClassName;
        /// <summary>
        /// クラス名
        /// </summary>
        public string ClassName
        {
            get { return _ClassName; }
            set
            {
                if (_ClassName != value)
                {
                    _ClassName = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region Bounds 変更通知プロパティ
        private Rectangle _Bounds;
        /// <summary>
        /// 位置と座標
        /// </summary>
        public Rectangle Bounds
        {
            get { return _Bounds; }
            set
            {
                if (_Bounds != value)
                {
                    _Bounds = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ControlSelectInfoViewModel()
        {
        }
    }
}
