using Livet;
using Livet.Messaging.Windows;
using System;
using System.Drawing;
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
    /// 範囲選択ウィンドウViewModel。
    /// </summary>
    public class RegionSelectionWindowViewModel : WindowViewModel
    {
        /// <summary>
        /// 選択範囲を取得します。
        /// </summary>
        public Rectangle? SelectedRegion { get; private set; }

        /// <summary>
        /// コントロール選択情報ViewModel
        /// </summary>
        public ControlSelectionInfoViewModel ControlSelectInfo { get; set; }

        /// <summary>
        /// スクリーンの範囲
        /// </summary>
        private Rectangle ScreenBounds { get; set; }

        /// <summary>
        /// スクリーンの原点
        /// </summary>
        private Point ScreenOrigin => ScreenBounds.Location;

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
        public RegionSelectionWindowViewModel()
        {
            this.ControlSelectInfo = new ControlSelectionInfoViewModel().AddTo(this);
            this.Subscribe(nameof(MousePoint), () =>
            {
                SetLinePoint(MousePoint);
                this.ControlSelectInfo.Update(this.MousePoint);
            }).AddTo(this);
        }

        /// <summary>
        /// 初期化処理。
        /// </summary>
        protected override void InitializeCore()
        {
            // ウィンドウに画面全体の範囲を設定する
            ScreenBounds = ScreenHelper.GetFullScreenBounds();
            this.Messenger.Raise(new SetWindowBoundsMessage
            {
                MessageKey = "Window.Bounds",
                Left = ScreenBounds.Left,
                Top = ScreenBounds.Top,
                Width = ScreenBounds.Width,
                Height = ScreenBounds.Height
            });

            // 初期化
            this.SendWindowAction(WindowAction.Active);
            this.ControlSelectInfo.Initialize(ScreenOrigin, System.Windows.Forms.Cursor.Position);

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
            this.MousePoint = new Point(point.X + ScreenOrigin.X, point.Y + ScreenOrigin.Y);
        }

        /// <summary>
        /// マウスアップ処理。
        /// </summary>
        /// <param name="e">イベント引数</param>
        public void OnMouseUp(MouseEventArgs e)
        {
            e.Handled = true;
            //var handle = this.controlSelector.SelectedHandle;
            //if (e.LeftButton != MouseButtonState.Released)
            //{
            //    handle = IntPtr.Zero;
            //}

            this.SelectRegion(ScreenHelper.GetFullScreenBounds());
        }

        /// <summary>
        /// キーダウン処理。
        /// </summary>
        /// <param name="e">イベント引数</param>
        public void OnKeyDown(KeyEventArgs e)
        {
            e.Handled = true;
            this.SelectRegion(null);
        }

        /// <summary>
        /// ウィンドウを非表示にしてコントロールを選択します。
        /// </summary>
        /// <param name="handle">選択したハンドル</param>
        private void SelectRegion(Rectangle? region)
        {
            this.SelectedRegion = region;
            this.SetVisibility(Visibility.Hidden);
            DispatcherHelper.UIDispatcher.Invoke(() => { }, DispatcherPriority.Background);
        }

        /// <summary>
        /// ウィンドウの表示状態を設定します。
        /// </summary>
        /// <param name="value">表示状態</param>
        private void SetVisibility(Visibility visibility)
        {
            this.Messenger.Raise(new SetVisibilityMessage
            {
                MessageKey = "Window.Visibility",
                Visibility = visibility
            });
        }

        /// <summary>
        /// 指定されたマウス座標に従い、ライン座標を設定します。
        /// </summary>
        /// <param name="point">マウス座標</param>
        private void SetLinePoint(Point point)
        {
            this.Messenger.Raise(new SetLinePointMessage
            {
                MessageKey = "HorizontalLine.Point",
                X1 = ScreenBounds.X,
                Y1 = point.Y - ScreenBounds.Y,
                X2 = ScreenBounds.Width,
                Y2 = point.Y - ScreenBounds.Y,
            });
            this.Messenger.Raise(new SetLinePointMessage
            {
                MessageKey = "VerticalLine.Point",
                X1 = point.X - ScreenBounds.X,
                Y1 = ScreenBounds.Y,
                X2 = point.X - ScreenBounds.X,
                Y2 = ScreenBounds.Height,
            });
        }
    }
}