using Livet;
using Livet.Messaging.Windows;
using System;
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
        /// <summary>
        /// スクリーンの原点。
        /// </summary>
        private Point screenOrigin;

        /// <summary>
        /// コントロール選択モデル。
        /// </summary>
        private ControlSelector controlSelector;

        /// <summary>
        /// 選択結果を取得します。
        /// </summary>
        public IntPtr? Result { get; private set; }

        /// <summary>
        /// コントロール選択情報ViewModel
        /// </summary>
        public ControlSelectionInfoViewModel ControlSelectInfo { get; set; }

        /// <summary>
        /// コントロール選択の通知オブジェクト
        /// </summary>
        private readonly Subject<IntPtr?> notifier;

        #region SelectedRegion 変更通知プロパティ

        private Rect _SelectedRegion;

        /// <summary>
        /// 選択範囲を取得または設定します。
        /// </summary>
        public Rect SelectedRegion
        {
            get { return this._SelectedRegion; }
            set
            {
                if (this._SelectedRegion != value)
                {
                    this._SelectedRegion = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region MousePoint 変更通知プロパティ

        private Point _MousePoint;

        /// <summary>
        /// マウス座標を取得または設定します。
        /// </summary>
        public Point MousePoint
        {
            get { return this._MousePoint; }
            set
            {
                if (this._MousePoint != value)
                {
                    this._MousePoint = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ControlSelectionWindowViewModel()
        {
            this.ControlSelectInfo = new ControlSelectionInfoViewModel().AddTo(this);
            this.controlSelector = new ControlSelector();
            this.controlSelector.Subscribe(nameof(this.controlSelector.SelectedHandle), () =>
            {
                var handle = this.controlSelector.SelectedHandle;
                if (handle == null) { return; }
                var bounds = InteropHelper.GetWindowSize(handle.Value);

                // コントロール情報の更新
                this.ControlSelectInfo.SetInfo(handle.Value, bounds);

                // ワールド座標に変換して選択範囲を設定する
                this.SelectedRegion = new Rect(
                    bounds.Left - this.screenOrigin.X,
                    bounds.Top - this.screenOrigin.Y,
                    bounds.Width,
                    bounds.Height);
            }).AddTo(this);

            // マウス座標変更イベントの購読
            this.Subscribe(nameof(this.MousePoint), () =>
            {
                this.controlSelector.Update(this.MousePoint);
                this.ControlSelectInfo.Update(this.MousePoint);
            }).AddTo(this);

            // 選択範囲をクリア、設定する
            this.notifier = new Subject<IntPtr?>();
            this.notifier
                .Do(x => this.SelectedRegion = new Rect(0, 0, 0, 0))
                .Delay(TimeSpan.FromMilliseconds(100))
                .Subscribe(x => this.SelectControl(x))
                .AddTo(this);
        }

        /// <summary>
        /// 初期化処理。
        /// </summary>
        protected override void InitializeCore()
        {
            // ウィンドウに画面全体の範囲を設定する
            var screenRect = ScreenHelper.GetFullScreenBounds();
            this.Messenger.Raise(new SetWindowBoundsMessage
            {
                MessageKey = "Window.Bounds",
                Left = screenRect.Left,
                Top = screenRect.Top,
                Width = screenRect.Width,
                Height = screenRect.Height
            });
            this.screenOrigin = screenRect.Location;

            // 初期化
            this.SendWindowAction(WindowAction.Active);
            this.controlSelector.Initialize();
            this.ControlSelectInfo.Initialize(this.screenOrigin, System.Windows.Forms.Cursor.Position);

            // マウス座標の設定
            this.SetMousePoint(System.Windows.Forms.Cursor.Position);
        }

        /// <summary>
        /// マウス移動処理。
        /// </summary>
        /// <param name="e">イベント引数</param>
        public void OnMouseMove(MouseEventArgs e)
        {
            var p = e.GetPosition(null);
            this.SetMousePoint(new Point((int)p.X, (int)p.Y));
        }

        /// <summary>
        /// マウス座標を設定します。
        /// </summary>
        /// <param name="point">ワールド座標のマウス座標</param>
        private void SetMousePoint(Point point)
        {
            this.MousePoint = new Point(point.X + this.screenOrigin.X, point.Y + this.screenOrigin.Y);
        }

        /// <summary>
        /// マウスアップ処理。
        /// </summary>
        /// <param name="e">イベント引数</param>
        public void OnMouseUp(MouseEventArgs e)
        {
            e.Handled = true;
            var handle = this.controlSelector.SelectedHandle;
            if (e.LeftButton != MouseButtonState.Released)
            {
                handle = null;
            }
            this.notifier.OnNext(handle);
        }

        /// <summary>
        /// キーダウン処理。
        /// </summary>
        /// <param name="e">イベント引数</param>
        public void OnKeyDown(KeyEventArgs e)
        {
            e.Handled = true;
            this.notifier.OnNext(null);
        }

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