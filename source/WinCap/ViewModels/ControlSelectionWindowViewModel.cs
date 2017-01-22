using Livet;
using Livet.Messaging;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WinCap.Interop;
using WinCap.Models;
using WinCap.Util.Mvvm;
using WinCap.ViewModels.Messages;
using Rectangle = System.Drawing.Rectangle;

namespace WinCap.ViewModels
{
    /// <summary>
    /// コントロール選択ウィンドウViewModel
    /// </summary>
    public class ControlSelectionWindowViewModel : ViewModel
    {
        /// <summary>
        /// 有効かどうかを示す値。
        /// </summary>
        private bool _enabled;

        /// <summary>
        /// ウィンドウの左端座標。
        /// </summary>
        private System.Drawing.Point _location;

        /// <summary>
        /// コントロール選択モデル。
        /// </summary>
        private ControlSelection _controlSelection;

        /// <summary>
        /// DPI倍率を取得または設定します。
        /// </summary>
        public Point DpiScaleFactor { get; set; } = new Point(1.0, 1.0);

        /// <summary>
        /// 選択コントロールのハンドルを取得します。
        /// </summary>
        public IntPtr SelectedHandle { get; private set; }

        /// <summary>
        /// コントロール選択情報ViewModel
        /// </summary>
        public ControlSelectionInfoViewModel ControlSelectInfo { get; set; }

        /// <summary>
        /// 初期化時に呼び出すアクション。
        /// </summary>
        public Action Initialized;

        /// <summary>
        /// コントロール選択時に呼び出すアクション。
        /// </summary>
        public Action Selected;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ControlSelectionWindowViewModel()
        {
            this.ControlSelectInfo = new ControlSelectionInfoViewModel().AddTo(this);
            this._controlSelection = new ControlSelection();
            this._controlSelection.Subscribe(nameof(this._controlSelection.SelectedHandle), () =>
            {
                var handle = this._controlSelection.SelectedHandle;
                var bounds = handle.GetWindowBounds();

                // コントロール情報の更新
                this.ControlSelectInfo.SetInfo(handle, bounds);

                // コントロール座標に変換して選択範囲を設定する
                this.Messenger.Raise(new SetRectangleBoundsMessage
                {
                    MessageKey = "Rectangle.Bounds",
                    Left = bounds.Left - this._location.X,
                    Top = bounds.Top - this._location.Y,
                    Width = bounds.Width,
                    Height = bounds.Height
                });
            }).AddTo(this);
        }

        /// <summary>
		/// <see cref="Window.ContentRendered"/>イベントが発生したときに
        /// Livet インフラストラクチャによって呼び出されます。
        /// </summary>
        public void Initialize()
        {
            // ウィンドウに画面全体の範囲を設定する
            Rectangle screenRect = ScreenHelper.GetFullScreenBounds();
            this.Messenger.Raise(new SetWindowBoundsMessage
            {
                MessageKey = "Window.Bounds",
                Left = screenRect.Left,
                Top = screenRect.Top,
                Width = screenRect.Width,
                Height = screenRect.Height
            });
            this._location = screenRect.Location;

            // 初期化
            this.Initialized?.Invoke();
            this._controlSelection.Initialize();
            this._enabled = true;
        }

        /// <summary>
        /// マウス移動時に呼ばれます。
        /// </summary>
        /// <param name="e">イベント引数</param>
        public void OnMouseMove(MouseEventArgs e)
        {
            if (this._enabled)
            {
                var point = e.GetPosition(null);
                var screenPoint = new System.Drawing.Point((int)point.X + this._location.X, (int)point.Y + this._location.Y);
                this._controlSelection.UpdateMousePoint(screenPoint);
                this.ControlSelectInfo.UpdateMousePoint(screenPoint);
            }
        }

        /// <summary>
        /// マウスアップ時に呼ばれます。
        /// </summary>
        /// <param name="e">イベント引数</param>
        public void OnMouseUp(MouseEventArgs e)
        {
            e.Handled = true;
            var handle = this._controlSelection.SelectedHandle;
            if (e.LeftButton != MouseButtonState.Released)
            {
                handle = IntPtr.Zero;
            }

            SelectedControl(handle);
        }

        /// <summary>
        /// キーダウン時に呼ばれます。
        /// </summary>
        /// <param name="e">イベント引数</param>
        public void OnKeyDown(KeyEventArgs e)
        {
            e.Handled = true;
            SelectedControl(IntPtr.Zero);
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
        /// ウィンドウを非表示にしてコントロールを選択します。
        /// </summary>
        /// <param name="handle">選択したハンドル</param>
        private void SelectedControl(IntPtr handle)
        {
            this.SelectedHandle = handle;
            this.SetVisibility(Visibility.Hidden);
            DispatcherHelper.UIDispatcher.Invoke(() => { }, DispatcherPriority.Background);
            if (handle != IntPtr.Zero)
            {
                this.Selected?.Invoke();
            }
            this._enabled = false;
        }
    }
}
