using Livet;
using WinCap.Models;

namespace WinCap.ViewModels
{
    /// <summary>
    /// コントロール選択情報ViewModel
    /// </summary>
    public class ControlSelectInfoViewModel : ViewModel
    {
        private ControlInfo _ControlInfo;

        /// <summary>
        /// コントロール情報
        /// </summary>
        public ControlInfo ControlInfo
        {
            get { return _ControlInfo; }
            set
            {
                if (_ControlInfo != value)
                {
                    _ControlInfo = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ControlSelectInfoViewModel()
        {
        }
    }
}
