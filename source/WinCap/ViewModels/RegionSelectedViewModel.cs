using Livet;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Point = System.Drawing.Point;

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
        /// <param name="mouseDown">マウスダウンイベント</param>
        /// <param name="mouseUp">マウスアップイベント</param>
        /// <param name="mouseMove">マウス移動イベント</param>
        public SelectedRegionViewModel()
        {
            //ReactiveProperty<MouseEventArgs> mouseDown, ReactiveProperty<MouseEventArgs> mouseUp, ReactiveProperty<Point> mouseMove
            //// ドラッグ中に選択領域を設定する
            //SelectedRegion = mouseDown
            //    .Where(e => e.LeftButton == MouseButtonState.Pressed)
            //    .SelectMany(_ => mouseMove)
            //    .TakeUntil(mouseUp)
            //    .Repeat()
            //    .Select(_ => CreateRegion(StartPoint.Value, CurrentPoint.Value))
            //    .Select(x => new Rect(x.X - ScreenOrigin.X, x.Y - ScreenOrigin.Y, x.Width, x.Height))
            //    .ToReactiveProperty(mode: ReactivePropertyMode.None)
            //    .AddTo(this);
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
    }
}
