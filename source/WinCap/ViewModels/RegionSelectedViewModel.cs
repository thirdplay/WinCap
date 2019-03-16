using Livet;
using Reactive.Bindings;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using WinCap.Interop;
using WinCap.Models;
using WpfUtility.Mvvm;

namespace WinCap.ViewModels
{
    /// <summary>
    /// 選択した領域のViewModel。
    /// </summary>
    public class SelectedRegionViewModel : ViewModel
    {
        /// <summary>
        /// 選択領域
        /// </summary>
        public ReactiveProperty<Rect> SelectedRegion { get; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="regionSelectionModel">領域選択Model</param>
        public SelectedRegionViewModel(RegionSelectionModel regionSelectionModel)
        {
            this.SelectedRegion = regionSelectionModel.SelectedRegion
                .Select(x => x.ToRect())
                .ToReactiveProperty(mode: ReactivePropertyMode.RaiseLatestValueOnSubscribe)
                .AddTo(this);
        }
    }
}
