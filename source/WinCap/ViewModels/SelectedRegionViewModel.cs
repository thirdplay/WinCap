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
using SelectionStatus = WinCap.Models.RectangleTracker.SelectionStatus;
using EventHolder = WinCap.ViewModels.RegionSelectionWindowViewModel.EventHolder;

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
        /// <param name="events">イベント保有者</param>
        /// <param name="tracker">矩形トラッカー</param>
        public SelectedRegionViewModel(EventHolder events, RectangleTracker tracker)
        {
            // 左ボタン押下時に選択開始する
            events.MouseDown
                .Where(e => e.LeftButton == MouseButtonState.Pressed)
                .Select(e => e.GetPosition(null).ToPoint().PointToScreen())
                .Subscribe(x => tracker.Start(x))
                .AddTo(this);

            // マウス移動時に現在座標を更新する
            events.MouseMove
                .Select(e => e.GetPosition(null).ToPoint().PointToScreen())
                .Subscribe(x => tracker.Update(x))
                .AddTo(this);

            // 左ボタンアップ時に選択終了する
            events.MouseUp
                .Where(e => e.LeftButton == MouseButtonState.Released)
                .Select(e => e.GetPosition(null).ToPoint().PointToScreen())
                .Subscribe(x => tracker.Stop(x))
                .AddTo(this);

            // キーダウン時に選択を中断する
            events.KeyDown
                .Do(e => e.Handled = true)
                .Subscribe(_ => tracker.Susspend())
                .AddTo(this);

            // 選択中の場合、選択領域を表示する
            SelectedRegion = tracker.SelectedRange
                .Where(_ => tracker.Status.Value == SelectionStatus.Selecting)
                .Select(x => x.ToRect())
                .ToReactiveProperty()
                .AddTo(this);

            // 選択状態に応じて選択領域とマウスカーソルの表示を制御する
            tracker.Status
                .Subscribe(s =>
                {
                    if (s == SelectionStatus.Selecting)
                    {
                        Mouse.OverrideCursor = Cursors.None;
                    }
                    else
                    {
                        Mouse.OverrideCursor = null;
                        SelectedRegion.Value = new Rect();
                    }
                })
                .AddTo(this);
        }
    }
}
