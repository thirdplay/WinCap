using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Drawing;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

namespace WinCap.Models
{
    /// <summary>
    /// 領域の選択機能を提供するModel。
    /// </summary>
    public class RegionSelectionModel : IDisposable
    {
        /// <summary>
        /// リソース解放管理
        /// </summary>
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        /// <summary>
        /// 選択領域
        /// </summary>
        public ReactiveProperty<Rectangle> SelectedRegion { get; }

        /// <summary>
        /// 現在のマウス座標
        /// </summary>
        public ReactiveProperty<Point> CurrentPoint { get; }

        /// <summary>
        /// 選択領域の始点
        /// </summary>
        public ReactiveProperty<Point> StartPoint { get; }

        /// <summary>
        /// 選択状態
        /// </summary>
        public ReactiveProperty<bool> IsSelecting { get; } = new ReactiveProperty<bool>();

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="mouseDown">マウスダウンイベント</param>
        /// <param name="mouseUp">マウスアップイベント</param>
        /// <param name="mouseMove">マウス移動イベント</param>
        public RegionSelectionModel(
            ReactiveProperty<MouseEventArgs> mouseDown,
            ReactiveProperty<MouseEventArgs> mouseUp,
            ReactiveProperty<Point> mouseMove)
        {
            // 左ボタン押下時に開始座標を更新する
            this.StartPoint = mouseDown
                .Where(e => e.LeftButton == MouseButtonState.Pressed)
                .Select(_ => CurrentPoint.Value)
                .ToReactiveProperty(mode: ReactivePropertyMode.None)
                .AddTo(this.Disposable);

            // マウス移動時に現在座標を更新する
            this.CurrentPoint = mouseMove
                .ToReactiveProperty(mode: ReactivePropertyMode.RaiseLatestValueOnSubscribe)
                .AddTo(this.Disposable);

            // ドラッグ中に選択領域を設定する
            this.SelectedRegion = mouseDown
                .Where(e => e.LeftButton == MouseButtonState.Pressed)
                .Do(_ => IsSelecting.Value = true)
                .SelectMany(_ => mouseMove)
                .TakeUntil(mouseUp)
                .Do(_ => IsSelecting.Value = false)
                .Repeat()
                .Select(_ => CreateRegion(StartPoint.Value, CurrentPoint.Value))
                .ToReactiveProperty(mode: ReactivePropertyMode.None)
                .AddTo(this.Disposable);
        }

        /// <summary>
        /// 選択領域をクリアします。
        /// </summary>
        public void Clear()
        {
            this.SelectedRegion.Value = new Rectangle();
        }

        /// <summary>
        /// 始点と終点から領域を作成します。
        /// </summary>
        /// <param name="p1">始点座標</param>
        /// <param name="p2">終点座標</param>
        /// <returns>領域</returns>
        private Rectangle CreateRegion(Point p1, Point p2)
        {
            return new Rectangle()
            {
                X = Math.Min(p1.X, p2.X),
                Y = Math.Min(p1.Y, p2.Y),
                Width = Math.Abs(p1.X - p2.X),
                Height = Math.Abs(p1.Y - p2.Y),
            };
        }

        /// <summary>
        /// このインスタンスによって使用されているリソースを全て破棄します。
        /// </summary>
        public void Dispose() => this.Disposable.Dispose();
    }
}
