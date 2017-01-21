using Livet;
using Livet.Messaging;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
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
        /// コントロール選択モデル。
        /// </summary>
        private ControlSelection _controlSelection;

        /// <summary>
        /// ウィンドウの左端座標を取得します。
        /// </summary>
        private System.Drawing.Point _location;

        /// <summary>
        /// 選択コントロールのハンドルを取得します。
        /// </summary>
        public IntPtr SelectedHandle { get; private set; } = IntPtr.Zero;

        /// <summary>
        /// コントロール選択情報ViewModel
        /// </summary>
        public ControlSelectionInfoViewModel ControlSelectInfo { get; set; }

        /// <summary>
        /// 初期化時に発生するイベント。
        /// </summary>
        public event EventHandler Initialized;

        /// <summary>
        /// コントロール選択時に発生するイベント。
        /// </summary>
        public event EventHandler Selected;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ControlSelectionWindowViewModel()
        {
            this.ControlSelectInfo = new ControlSelectionInfoViewModel().AddTo(this);
            this.Initialized += (s, e) => this.ControlSelectInfo.Initialize();
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
            this._controlSelection = new ControlSelection();

            // 初期化イベントを発火
            this.Initialized?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// マウス移動時に呼ばれます。
        /// </summary>
        /// <param name="e">イベント引数</param>
        public void OnMouseMove(MouseEventArgs e)
        {
            if (this._controlSelection == null)
            {
                return;
            }

            // マウス座標をスクリーン座標に変換する
            var mousePoint = e.GetPosition(null);
            var point = new System.Drawing.Point((int)mousePoint.X + this._location.X, (int)mousePoint.Y + this._location.Y);

            // ウィンドウハンドルの取得
            var handle = this._controlSelection.GetWindowHandle(point);
            if (this.SelectedHandle != handle)
            {
                this.SelectedHandle = handle;
                var bounds = InteropHelper.GetWindowBounds(handle);

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
            }

            // 選択コントロール情報を更新する
            this.ControlSelectInfo.Update();
        }

        /// <summary>
        /// マウスアップ時に呼ばれます。
        /// </summary>
        /// <param name="e">イベント引数</param>
        public void OnMouseUp(MouseEventArgs e)
        {
            e.Handled = true;
            var handle = this.SelectedHandle;
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
        /// ウィンドウを閉じます。
        /// </summary>
        public void Close()
        {
            this.Messenger.Raise(new InteractionMessage("Window.Close"));
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
        /// コントロールを選択し、ウィンドウを閉じます。
        /// </summary>
        /// <param name="handle">選択したハンドル</param>
        private void SelectedControl(IntPtr handle)
        {
            this.SelectedHandle = handle;
            this.SetVisibility(Visibility.Hidden);
            BackgroundInvoke();
            if (handle != IntPtr.Zero)
            {
                this.Selected?.Invoke(this, EventArgs.Empty);
            }
            this.Close();
        }

        /// <summary>
        /// バックグラインド処理を呼び出し、他の操作の完了を待ちます。
        /// </summary>
        private void BackgroundInvoke()
        {
            DispatcherHelper.UIDispatcher.Invoke(() => { }, DispatcherPriority.Background);
        }
    }
}
