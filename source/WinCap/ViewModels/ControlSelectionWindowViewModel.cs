using Livet;
using Livet.Messaging;
using System;
using System.Windows.Input;
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
        /// コンストラクタ
        /// </summary>
        public ControlSelectionWindowViewModel()
        {
            this.ControlSelectInfo = new ControlSelectionInfoViewModel().AddTo(this);
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

            // コントロール選択情報の初期化
            this.ControlSelectInfo.Initialize();
        }

        /// <summary>
        /// マウス移動イベント
        /// </summary>
        /// <param name="e">イベント引数</param>
        public void MouseMove(MouseEventArgs e)
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
        /// マウスアップイベント
        /// </summary>
        /// <param name="e">イベント引数</param>
        public void MouseUp(MouseEventArgs e)
        {
            e.Handled = true;
            var handle = IntPtr.Zero;
            if (e.LeftButton != MouseButtonState.Released)
            {
                this.SelectedHandle = IntPtr.Zero;
            }
            this.Messenger.Raise(new InteractionMessage("Window.Close"));
        }

        /// <summary>
        /// キーダウンイベント
        /// </summary>
        /// <param name="e"></param>
        public void KeyDown(KeyEventArgs e)
        {
            e.Handled = true;
            this.SelectedHandle = IntPtr.Zero;
            this.Messenger.Raise(new InteractionMessage("Window.Close"));
        }
    }
}
