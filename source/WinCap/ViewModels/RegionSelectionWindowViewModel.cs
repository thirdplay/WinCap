﻿using Livet;
using Livet.Messaging.Windows;
using System;
using System.Drawing;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
    /// 領域選択ウィンドウViewModel。
    /// </summary>
    public class RegionSelectionWindowViewModel : WindowViewModel
    {
        /// <summary>
        /// 選択結果を取得します。
        /// </summary>
        public Rectangle? Result { get; private set; }

        /// <summary>
        /// 領域選択情報ViewModel
        /// </summary>
        public RegionSelectionInfoViewModel RegionSelectionInfo { get; set; }

        /// <summary>
        /// スクリーンの範囲
        /// </summary>
        private Rectangle screenBounds;

        /// <summary>
        /// スクリーンの原点
        /// </summary>
        private Point ScreenOrigin => this.screenBounds.Location;

        /// <summary>
        /// 選択領域の始点
        /// </summary>
        private Point? startPoint;

        /// <summary>
        /// 領域選択時の処理シーケンス
        /// </summary>
        private readonly Subject<Rectangle?> notifier;

        #region SelectedRegion 変更通知プロパティ

        private Rect _SelectedRegion;

        /// <summary>
        /// 選択領域を取得または設定します。
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
        public RegionSelectionWindowViewModel()
        {
            this.RegionSelectionInfo = new RegionSelectionInfoViewModel().AddTo(this);

            this.Subscribe(nameof(MousePoint), () =>
            {
                Rectangle? region = null;
                if (this.startPoint.HasValue)
                {
                    // ドラッグ中の場合、ワールド座標に変換して選択領域を設定する
                    region = GetSelectedRegion(this.startPoint.Value, this.MousePoint);
                    this.SelectedRegion = new Rect(
                        region.Value.X - this.ScreenOrigin.X,
                        region.Value.Y - this.ScreenOrigin.Y,
                        region.Value.Width,
                        region.Value.Height);
                }

                // 領域選択情報の更新
                this.RegionSelectionInfo.Update(this.MousePoint, this.startPoint, region);
            }).AddTo(this);

            // 領域選択時の処理シーケンスの生成
            this.notifier = new Subject<Rectangle?>();
            this.notifier
                .Do(x => this.SelectedRegion = new Rect(0, 0, 0, 0))
                .Delay(TimeSpan.FromMilliseconds(100))
                .Subscribe(x =>
                {
                    DispatcherHelper.UIDispatcher.Invoke(() => Mouse.OverrideCursor = null);
                    this.SelectRegion(x);
                })
                .AddTo(this);
        }

        /// <summary>
        /// 初期化処理。
        /// </summary>
        protected override void InitializeCore()
        {
            // ウィンドウに画面全体の領域を設定する
            this.screenBounds = ScreenHelper.GetFullScreenBounds();
            this.Messenger.Raise(new SetWindowBoundsMessage
            {
                MessageKey = "Window.Bounds",
                Left = this.screenBounds.Left,
                Top = this.screenBounds.Top,
                Width = this.screenBounds.Width,
                Height = this.screenBounds.Height
            });

            // 初期化
            this.SendWindowAction(WindowAction.Active);
            this.RegionSelectionInfo.Initialize(this.ScreenOrigin);

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
                // 始点選択完了→終点選択開始
                Mouse.OverrideCursor = Cursors.None;
                this.startPoint = this.MousePoint;
            }
        }

        /// <summary>
        /// マウスアップ処理。
        /// </summary>
        /// <param name="e">イベント引数</param>
        public void OnMouseUp(MouseEventArgs e)
        {
            Rectangle region = new Rectangle(0, 0, 0, 0);
            e.Handled = true;
            if (e.LeftButton == MouseButtonState.Released)
            {
                region = GetSelectedRegion(this.startPoint.Value, this.MousePoint);
            }
            if (region.Width == 0 || region.Height == 0)
            {
                // 幅や高さが0の場合は始点選択からやり直し
                Mouse.OverrideCursor = null;
                this.startPoint = null;
                return;
            }
            notifier.OnNext(region);
        }

        /// <summary>
        /// キーダウン処理。
        /// </summary>
        /// <param name="e">イベント引数</param>
        public void OnKeyDown(KeyEventArgs e)
        {
            e.Handled = true;
            notifier.OnNext(null);
        }

        /// <summary>
        /// ウィンドウを非表示にしてコントロールを選択します。
        /// </summary>
        /// <param name="handle">選択したハンドル</param>
        private void SelectRegion(Rectangle? region)
        {
            this.startPoint = null;
            this.SelectedRegion = new Rect();
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
    }
}