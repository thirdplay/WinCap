using Livet;
using Livet.Messaging.Windows;
using Reactive.Bindings;
using System;
using System.Diagnostics;
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
    /// コントロール選択ウィンドウViewModel
    /// </summary>
    public class ControlSelectionWindowViewModel : WindowViewModel
    {
        #region イベントホルダー
        /// <summary>
        /// イベント保有者
        /// </summary>
        public class EventHolder
        {
            /// <summary>
            /// マウスアップ
            /// </summary>
            public ReactiveProperty<MouseEventArgs> MouseUp { get; } = new ReactiveProperty<MouseEventArgs>(mode: ReactivePropertyMode.None);

            /// <summary>
            /// マウス移動
            /// </summary>
            public ReactiveProperty<MouseEventArgs> MouseMove { get; } = new ReactiveProperty<MouseEventArgs>(mode: ReactivePropertyMode.None);

            /// <summary>
            /// キーダウン
            /// </summary>
            public ReactiveProperty<KeyEventArgs> KeyDown { get; } = new ReactiveProperty<KeyEventArgs>(mode: ReactivePropertyMode.None);
        }
        #endregion

        /// <summary>
        /// ウィンドウを常に手前に表示するか否か返す。
        /// </summary>
        /// <remarks>デバッグ中は常に手前には表示しない</remarks>
        public bool TopMost => Debugger.IsAttached;

        /// <summary>
        /// コントロール選択者
        /// </summary>
        private ControlSelector Selector { get; }

        /// <summary>
        /// 選択したコントロールのViewModel
        /// </summary>
        public SelectedControlViewModel SelectedViewModel { get; }

        /// <summary>
        /// 選択結果を取得します。
        /// </summary>
        public IntPtr? Result { get; private set; }

        /// <summary>
        /// コントロール選択情報ViewModel
        /// </summary>
        public ControlSelectionInfoViewModel SelectionInfoViewModel { get; set; }

        /// <summary>
        /// イベント
        /// </summary>
        public EventHolder Events { get; set; } = new EventHolder();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ControlSelectionWindowViewModel()
        {
            //this.ControlSelectionInfo = new ControlSelectionInfoViewModel().AddTo(this);
            //this.controlSelector = new ControlSelector();
            //this.controlSelector.Subscribe(nameof(this.controlSelector.SelectedHandle), () =>
            //{
            //    var handle = this.controlSelector.SelectedHandle;
            //    if (handle == null) { return; }
            //    var bounds = InteropHelper.GetWindowSize(handle.Value);

            //    // コントロール情報の更新
            //    this.ControlSelectionInfo.UpdateInfo(handle.Value, bounds);

            //    // ワールド座標に変換して選択範囲を設定する
            //    this.SelectedRegion = new Rect(
            //        bounds.Left - this.screenOrigin.X,
            //        bounds.Top - this.screenOrigin.Y,
            //        bounds.Width,
            //        bounds.Height);
            //}).AddTo(this);

            //// マウス座標変更イベントの購読
            //this.Subscribe(nameof(this.MousePoint), () =>
            //{
            //    this.controlSelector.Update(this.MousePoint);
            //    this.ControlSelectionInfo.Update(this.MousePoint);
            //}).AddTo(this);

            //// コントロール選択時の処理シーケンスの生成
            //this.notifier = new Subject<IntPtr?>();
            //this.notifier
            //    .Do(x => this.SelectedRegion = new Rect(0, 0, 0, 0))
            //    .Delay(TimeSpan.FromMilliseconds(100))
            //    .Subscribe(x => this.SelectControl(x))
            //    .AddTo(this);

            // Model/ViewModelの生成
            Selector = new ControlSelector();
            SelectedViewModel = new SelectedControlViewModel(Events, Selector).AddTo(this);

            //// 選択完了時にウィンドウを非表示にする
            //this.Selector.Status
            //    .Where(x => x == RectangleTracker.SelectionStatus.Completed)
            //    .Delay(TimeSpan.FromMilliseconds(100))
            //    .ObserveOn(DispatcherHelper.UIDispatcher)
            //    .Subscribe(x => {
            //        if (!Tracker.IsEmptySelectedRange)
            //        {
            //            this.Result = Tracker.SelectedRange.Value;
            //        }
            //        this.Messenger.Raise(new SetVisibilityMessage
            //        {
            //            MessageKey = "Window.Visibility",
            //            Visibility = Visibility.Hidden
            //        });
            //    })
            //    .AddTo(this);
        }

        /// <summary>
        /// 初期化処理。
        /// </summary>
        protected override void InitializeCore()
        {
            this.Result = null;

            // ウィンドウに画面全体の範囲を設定する
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
            Selector.Initialize();
            this.SendWindowAction(WindowAction.Active);
            //this.controlSelector.Initialize();
            //this.ControlSelectionInfo.Initialize(new Point());
        }

        ///// <summary>
        ///// マウス移動処理。
        ///// </summary>
        ///// <param name="e">イベント引数</param>
        //public void OnMouseMove(MouseEventArgs e)
        //{
        //    var p = e.GetPosition(null);
        //    this.SetMousePoint(new Point((int)p.X, (int)p.Y));
        //}

        ///// <summary>
        ///// マウス座標を設定します。
        ///// </summary>
        ///// <param name="point">ワールド座標のマウス座標</param>
        //private void SetMousePoint(Point point)
        //{
        //    this.MousePoint = new Point(point.X + this.screenOrigin.X, point.Y + this.screenOrigin.Y);
        //}

        ///// <summary>
        ///// マウスアップ処理。
        ///// </summary>
        ///// <param name="e">イベント引数</param>
        //public void OnMouseUp(MouseEventArgs e)
        //{
        //    e.Handled = true;
        //    var handle = this.controlSelector.SelectedHandle;
        //    if (e.LeftButton != MouseButtonState.Released)
        //    {
        //        handle = null;
        //    }
        //    this.notifier.OnNext(handle);
        //}

        ///// <summary>
        ///// キーダウン処理。
        ///// </summary>
        ///// <param name="e">イベント引数</param>
        //public void OnKeyDown(KeyEventArgs e)
        //{
        //    e.Handled = true;
        //    this.notifier.OnNext(null);
        //}

        /// <summary>
        /// ウィンドウを非表示にしてコントロールを選択します。
        /// </summary>
        /// <param name="handle">選択したハンドル</param>
        private void SelectControl(IntPtr? handle)
        {
            this.Result = handle;
            this.Messenger.Raise(new SetVisibilityMessage
            {
                MessageKey = "Window.Visibility",
                Visibility = Visibility.Hidden
            });
            DispatcherHelper.UIDispatcher.Invoke(() => { }, DispatcherPriority.Background);
        }
    }
}