using Livet;
using Livet.Messaging.Windows;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WinCap.Capturers;
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
        private Point ScreenOrigin => this.ScreenBounds.Location;

        /// <summary>
        /// 選択範囲の始点
        /// </summary>
        public Point? StartPoint { get; set; }

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
                this.ControlSelectInfo.Update(this.MousePoint);
                if (this.StartPoint.HasValue)
                {
                    // 選択範囲を設定する
                    this.SetSelectedRegion(GetSelectedRegion(this.StartPoint.Value, this.MousePoint));
                }
            }).AddTo(this);
        }

        /// <summary>
        /// 初期化処理。
        /// </summary>
        protected override void InitializeCore()
        {
            // ウィンドウに画面全体の範囲を設定する
            this.ScreenBounds = ScreenHelper.GetFullScreenBounds();
            this.Messenger.Raise(new SetWindowBoundsMessage
            {
                MessageKey = "Window.Bounds",
                Left = this.ScreenBounds.Left,
                Top = this.ScreenBounds.Top,
                Width = this.ScreenBounds.Width,
                Height = this.ScreenBounds.Height
            });

            // 初期化
            this.SendWindowAction(WindowAction.Active);
            this.ControlSelectInfo.Initialize(this.ScreenOrigin, System.Windows.Forms.Cursor.Position);

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
            this.MousePoint = new Point(point.X + this.ScreenOrigin.X, point.Y + this.ScreenOrigin.Y);
        }

        /// <summary>
        /// マウスダウン処理。
        /// </summary>
        /// <param name="e">イベント引数</param>
        public void OnMouseDown(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.StartPoint = this.MousePoint;
            }
        }

        /// <summary>
        /// マウスアップ処理。
        /// </summary>
        /// <param name="e">イベント引数</param>
        public void OnMouseUp(MouseEventArgs e)
        {
            Rectangle? region = null;
            e.Handled = true;
            if (e.LeftButton == MouseButtonState.Released)
            {
                region = GetSelectedRegion(this.StartPoint.Value, this.MousePoint);
                this.StartPoint = null;
            }
            this.ClearSelectedRegion();
            this.SelectRegion(region);
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
        /// 選択範囲を設定します。
        /// </summary>
        /// <param name="region"></param>
        private void SetSelectedRegion(Rectangle region)
        {
            this.Messenger.Raise(new SetRectangleBoundsMessage
            {
                MessageKey = "Rectangle.Bounds",
                Left = region.X - this.ScreenOrigin.X,
                Top = region.Y - this.ScreenOrigin.Y,
                Width = region.Width,
                Height = region.Height,
            });
        }

        /// <summary>
        /// 選択範囲をクリアします。
        /// </summary>
        private void ClearSelectedRegion()
        {
            this.Messenger.Raise(new SetRectangleBoundsMessage
            {
                MessageKey = "Rectangle.Bounds",
                Width = 0,
                Height = 0,
            });
            DispatcherHelper.UIDispatcher.Invoke(() => { }, DispatcherPriority.Background);
        }

        /// <summary>
        /// 選択範囲を取得します。
        /// </summary>
        /// <param name="p1">始点座標</param>
        /// <param name="p2">終点座標</param>
        /// <returns>選択範囲</returns>
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
    }
}