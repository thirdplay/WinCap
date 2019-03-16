using Livet;
using Livet.Messaging.Windows;
using Reactive.Bindings;
using Reactive.Bindings.Interactivity;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WinCap.Capturers;
using WinCap.Interop;
using WinCap.Models;
using WinCap.ViewModels.Messages;
using WpfUtility.Mvvm;
using Point = System.Drawing.Point;

namespace WinCap.ViewModels
{
    /// <summary>
    /// 領域選択ウィンドウViewModel。
    /// </summary>
    public class RegionSelectionWindowViewModel : WindowViewModel
    {
        /// <summary>
        /// ウィンドウを常に手前に表示するか否か返す。
        /// </summary>
        /// <remarks>デバッグ中は常に手前には表示しない</remarks>
        public bool TopMost => Debugger.IsAttached;

        /// <summary>
        /// 選択結果を取得します。
        /// </summary>
        public Rectangle? Result { get; private set; }

        /// <summary>
        /// 選択した領域のViewModel
        /// </summary>
        public SelectedRegionViewModel SelectedViewModel { get; }

        /// <summary>
        /// 領域選択Model
        /// </summary>
        public RegionSelectionModel SelectionModel { get; }

        /// <summary>
        /// 領域選択情報ViewModel
        /// </summary>
        public RegionSelectionInfoViewModel RegionSelectionInfo { get; }

        /// <summary>
        /// 選択領域
        /// </summary>
        public ReactiveProperty<Rect> SelectedRegion { get; }

        /// <summary>
        /// マウスダウン
        /// </summary>
        public ReactiveProperty<MouseEventArgs> MouseDown { get; } = new ReactiveProperty<MouseEventArgs>(mode: ReactivePropertyMode.None);

        /// <summary>
        /// マウスアップ
        /// </summary>
        public ReactiveProperty<MouseEventArgs> MouseUp { get; } = new ReactiveProperty<MouseEventArgs>(mode: ReactivePropertyMode.None);

        /// <summary>
        /// マウス移動
        /// </summary>
        public ReactiveProperty<Point> MouseMove { get; } = new ReactiveProperty<Point>(mode: ReactivePropertyMode.None);

        /// <summary>
        /// 現在のマウス座標
        /// </summary>
        public ReactiveProperty<Point> CurrentPoint { get; }

        /// <summary>
        /// 選択領域の始点
        /// </summary>
        public ReactiveProperty<Point> StartPoint { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RegionSelectionWindowViewModel()
        {
            //this.Subscribe(nameof(MousePoint), () =>
            //{
            //    Rectangle? region = null;
            //    if (this.startPoint.HasValue)
            //    {
            //        // ドラッグ中の場合、ワールド座標に変換して選択領域を設定する
            //        region = GetSelectedRegion(this.startPoint.Value, this.MousePoint);
            //        this.SelectedRegion = new Rect(
            //            region.Value.X - this.ScreenOrigin.X,
            //            region.Value.Y - this.ScreenOrigin.Y,
            //            region.Value.Width,
            //            region.Value.Height);
            //    }

            //    // 領域選択情報の更新
            //    this.RegionSelectionInfo.Update(this.MousePoint, this.startPoint, region);
            //}).AddTo(this);

            //// 領域選択時の処理シーケンスの生成
            //this.notifier = new Subject<Rectangle?>();
            //this.notifier
            //    .Do(x => this.SelectedRegion = new Rect())
            //    .Delay(TimeSpan.FromMilliseconds(100))
            //    .Subscribe(x =>
            //    {
            //        DispatcherHelper.UIDispatcher.Invoke(() => Mouse.OverrideCursor = null);
            //        this.SelectRegion(x);
            //    })
            //    .AddTo(this);

            //// 左ボタン押下時に開始座標を更新する
            //this.StartPoint = this.MouseDown
            //    .Where(e => e.LeftButton == MouseButtonState.Pressed)
            //    .Select(_ => CurrentPoint.Value)
            //    .ToReactiveProperty(mode: none);

            //// マウス移動時に現在座標を更新する
            //this.CurrentPoint = this.MouseMove
            //    .ToReactiveProperty(mode: ReactivePropertyMode.RaiseLatestValueOnSubscribe)
            //    .AddTo(this);

            //// ドラッグ中に選択範囲を設定する
            //this.SelectedRegion = this.MouseDown
            //    .Where(e => e.LeftButton == MouseButtonState.Pressed)
            //    //.Do(_ => Mouse.OverrideCursor = Cursors.None)
            //    .SelectMany(_ => MouseMove)
            //    .TakeUntil(MouseUp)
            //    .Repeat()
            //    .Select(_ => GetSelectedRegion(StartPoint.Value, CurrentPoint.Value))
            //    //.Do(x => x.Location = x.Location.PointToScreen())
            //    .Select(x => x.ToRect())
            //    .ToReactiveProperty(mode: none)
            //    .AddTo(this);

            //// 左ボタンリリース時に選択範囲を設定する
            //this.MouseUp
            //    .Where(e => e.LeftButton == MouseButtonState.Released)
            //    .Select(_ => GetSelectedRegion(StartPoint.Value, CurrentPoint.Value))
            //    .Do(x => this.SelectedRegion.Value = new Rect())
            //    .Delay(TimeSpan.FromMilliseconds(100))
            //    .ObserveOn(DispatcherHelper.UIDispatcher)
            //    .Subscribe(x =>
            //    {
            //        Mouse.OverrideCursor = null;
            //        if (x.Width == 0 || x.Height == 0)
            //        {
            //            // 幅や高さが0の場合は始点選択からやり直し
            //            this.StartPoint.Value = new Point();
            //        }
            //        else
            //        {
            //            this.SelectRegion(x);
            //        }
            //    })
            //    .AddTo(this);

            this.RegionSelectionInfo = new RegionSelectionInfoViewModel().AddTo(this);

            this.SelectionModel = new RegionSelectionModel(MouseDown, MouseUp, MouseMove).AddTo(this);
            this.SelectedViewModel = new SelectedRegionViewModel(SelectionModel).AddTo(this);
        }

        /// <summary>
        /// 初期化処理。
        /// </summary>
        protected override void InitializeCore()
        {
            // ウィンドウに画面全体の領域を設定する
            var screenBounds = ScreenHelper.GetFullScreenBounds();
            this.Messenger.Raise(new SetWindowBoundsMessage
            {
                MessageKey = "Window.Bounds",
                Left = screenBounds.Left,
                Top = screenBounds.Top,
                Width = screenBounds.Width,
                Height = screenBounds.Height
            });

            // 初期化
            this.SendWindowAction(WindowAction.Active);
            this.RegionSelectionInfo.Initialize(new Point());
        }

        /// <summary>
        /// マウスアップ処理。
        /// </summary>
        /// <param name="e">イベント引数</param>
        public void OnMouseUp(MouseEventArgs e)
        {
            //Rectangle region = new Rectangle(0, 0, 0, 0);
            //e.Handled = true;
            //if (e.LeftButton == MouseButtonState.Released)
            //{
            //    region = GetSelectedRegion(this.startPoint.Value, this.MousePoint);
            //}
            //if (region.Width == 0 || region.Height == 0)
            //{
            //    // 幅や高さが0の場合は始点選択からやり直し
            //    Mouse.OverrideCursor = null;
            //    this.startPoint = null;
            //    return;
            //}
            //notifier.OnNext(region);
        }

        /// <summary>
        /// キーダウン処理。
        /// </summary>
        /// <param name="e">イベント引数</param>
        public void OnKeyDown(KeyEventArgs e)
        {
            e.Handled = true;
            //notifier.OnNext(null);
        }

        /// <summary>
        /// ウィンドウを非表示にしてコントロールを選択します。
        /// </summary>
        /// <param name="handle">選択したハンドル</param>
        private void SelectRegion(Rectangle? region)
        {
            this.StartPoint.Value = new Point();
            this.SelectedRegion.Value = new Rect();
            this.Result = region;
            this.Messenger.Raise(new SetVisibilityMessage
            {
                MessageKey = "Window.Visibility",
                Visibility = Visibility.Hidden
            });
            DispatcherHelper.UIDispatcher.Invoke(() => { }, DispatcherPriority.Background);
        }

        /// <summary>
        /// 選択領域を取得します。
        /// </summary>
        /// <param name="p1">始点座標</param>
        /// <param name="p2">終点座標</param>
        /// <returns>選択領域</returns>
        private Rectangle GetSelectedRegion(Point p1, Point p2)
        {
            return new Rectangle()
            {
                X = Math.Min(p1.X, p2.X),
                Y = Math.Min(p1.Y, p2.Y),
                Width = Math.Abs(p1.X - p2.X),
                Height = Math.Abs(p1.Y - p2.Y),
            };
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Console.WriteLine("Dispose:RegionSelectionWindowViewModel");
        }
    }

    /// <summary>
    /// マウスイベントをマウス座標に変換するコンバーター。
    /// </summary>
    public class MouseEventToPointConverter : ReactiveConverter<dynamic, Point>
    {
        protected override IObservable<Point> OnConvert(IObservable<dynamic> source)
        {
            return source
                .Select(x => x.GetPosition(null))
                .Select(x => new Point((int)x.X, (int)x.Y));
        }
    }
}