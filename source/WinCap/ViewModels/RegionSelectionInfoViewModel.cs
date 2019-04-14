using Livet;
using Reactive.Bindings;
using System;
using System.Drawing;
using System.Reactive.Linq;
using System.Windows.Forms;
using WinCap.Models;
using WinCap.ViewModels.Messages;
using WpfUtility.Mvvm;
using Visibility = System.Windows.Visibility;

namespace WinCap.ViewModels
{
    /// <summary>
    /// 領域選択情報ViewModel
    /// </summary>
    public class RegionSelectionInfoViewModel : ViewModel
    {
        /// <summary>
        /// 領域選択情報パネルのX座標
        /// </summary>
        public ReactiveProperty<double> Left { get; } = new ReactiveProperty<double>();

        /// <summary>
        /// 領域選択情報パネルのY座標
        /// </summary>
        public ReactiveProperty<double> Top { get; } = new ReactiveProperty<double>();

        /// <summary>
        /// 領域選択情報パネルの横幅
        /// </summary>
        public ReactiveProperty<double> Width { get; } = new ReactiveProperty<double>();

        /// <summary>
        /// 領域選択情報パネルの高さ
        /// </summary>
        public ReactiveProperty<double> Height { get; } = new ReactiveProperty<double>();

        /// <summary>
        /// 始点
        /// </summary>
        public ReactiveProperty<Point> StartPoint { get; } = new ReactiveProperty<Point>();

        /// <summary>
        /// 終点
        /// </summary>
        public ReactiveProperty<Point> EndPoint { get; } = new ReactiveProperty<Point>();

        /// <summary>
        /// サイズ
        /// </summary>
        public ReactiveProperty<Size> Size { get; } = new ReactiveProperty<Size>();

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="panel">表示位置制御パネル</param>
        /// <param name="tracker">矩形トラッカー</param>
        public RegionSelectionInfoViewModel(SwitchablePanel panel, RectangleTracker tracker) : base()
        {
            // 領域選択情報パネルの位置とサイズを反映する
            Left = panel.Location
                .Select(x => (double)x.X)
                .ToReactiveProperty()
                .AddTo(this);
            Top = panel.Location
                .Select(x => (double)x.Y)
                .ToReactiveProperty()
                .AddTo(this);
            Width = panel.Size
                .Select(x => (double)x.Width)
                .ToReactiveProperty()
                .AddTo(this);
            Height = panel.Size
                .Select(x => (double)x.Height)
                .ToReactiveProperty()
                .AddTo(this);

            // マウス座標変更時にパネルを更新する
            tracker.CurrentPoint
                .Subscribe(x => panel.Update(x))
                .AddTo(this);

            // 始点/終点/サイズ変更時に反映する
            StartPoint = tracker.CurrentPoint
                .Select(x => tracker.Status.Value == RectangleTracker.SelectionStatus.Selecting
                    ? tracker.StartPoint.Value
                    : x)
                .ToReactiveProperty()
                .AddTo(this);
            EndPoint = tracker.CurrentPoint
                .Select(x => tracker.Status.Value == RectangleTracker.SelectionStatus.Selecting
                    ? x
                    : new Point())
                .ToReactiveProperty()
                .AddTo(this);
            Size = tracker.SelectedRange
                .Select(x => x.Size)
                .ToReactiveProperty()
                .AddTo(this);
        }

        /// <summary>
        /// 初期化。
        /// </summary>
        /// <param name="screenOrigin">スクリーンの原点</param>
        public void Initialize()
        {
            this.Messenger.Raise(new SetVisibilityMessage
            {
                MessageKey = "Window.Visibility",
                Visibility = Visibility.Visible
            });
        }

        ///// <summary>
        ///// 更新処理。
        ///// </summary>
        ///// <param name="mousePoint">マウス座標</param>
        ///// <param name="startPoint">始点</param>
        ///// <param name="region">選択領域</param>
        //public void Update(Point mousePoint, Point? startPoint, Size? size)
        //{
        //    base.Update(mousePoint);
        //    if (startPoint.HasValue && size.HasValue)
        //    {
        //        this.StartPoint = startPoint.Value;
        //        this.EndPoint = mousePoint;
        //        this.Size = size.Value;
        //    }
        //    else
        //    {
        //        this.StartPoint = mousePoint;
        //        this.EndPoint = new Point();
        //        this.Size = new Size();
        //    }
        //}
    }
}