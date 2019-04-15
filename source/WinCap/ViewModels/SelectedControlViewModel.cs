using Livet;
using Reactive.Bindings;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using WinCap.Interop;
using WinCap.Models;
using WpfUtility.Mvvm;
using EventHolder = WinCap.ViewModels.ControlSelectionWindowViewModel.EventHolder;
using System.Drawing;

namespace WinCap.ViewModels
{
    /// <summary>
    /// 選択したコントロールのViewModel。
    /// </summary>
    public class SelectedControlViewModel : ViewModel
    {
        /// <summary>
        /// 選択領域
        /// </summary>
        public ReactiveProperty<Rect> SelectedRegion { get; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="events">イベント保有者</param>
        /// <param name="selector">コントロール選択者</param>
        public SelectedControlViewModel(EventHolder events, ControlSelector selector)
        {
            // マウス移動時に現在座標を更新する
            events.MouseMove
                .Select(e => e.GetPosition(null).ToPoint().PointToScreen())
                .Subscribe(x => selector.Update(x))
                .AddTo(this);

            // 選択ハンドル変更時に選択範囲を更新する
            SelectedRegion = selector.SelectedHandle
                .Select(x => x != null ? InteropHelper.GetWindowSize(x.Value) : new Rectangle())
                .Select(x => x.ToRect())
                .ToReactiveProperty()
                .AddTo(this);
        }
    }
}
