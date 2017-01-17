using Livet;
using Livet.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
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
        /// ウィンドウハンドルリスト
        /// </summary>
        private List<IntPtr> _handleList = new List<IntPtr>();

        /// <summary>
        /// スクリーンの座標
        /// </summary>
        private System.Drawing.Point _screenLocation;

        /// <summary>
        /// 選択中のコントロール情報
        /// </summary>
        private ControlInfo _selectControlInfo = ControlInfo.Empty;

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
            // 表示中のウィンドウハンドルリストを取得する
            this._handleList = WindowHelper.GetHandles();

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
            this._screenLocation = screenRect.Location;

            // コントロール選択情報の初期化
            this.ControlSelectInfo.Initialize();
        }

        /// <summary>
        /// マウス移動イベント
        /// </summary>
        /// <param name="e">イベント引数</param>
        public void MouseMove(MouseEventArgs e)
        {
            // スクリーン座標の取得
            var mousePoint = e.GetPosition(null);
            var p = new System.Drawing.Point((int)mousePoint.X + this._screenLocation.X, (int)mousePoint.Y + this._screenLocation.Y);

            // マウスカーソル上にあるウィンドウの情報を取得する
            ControlInfo selectControlInfo = ControlInfo.Empty;
            foreach (IntPtr handle in this._handleList)
            {
                Rectangle bounds = InteropHelper.GetWindowBounds(handle);
                if (bounds != Rectangle.Empty && bounds.Contains(p))
                {
                    selectControlInfo = new ControlInfo(handle, InteropHelper.GetClassName(handle), bounds);
                    break;
                }
            }

            UpdateSelectControlInfo(selectControlInfo);
        }

        /// <summary>
        /// マウスアップイベント
        /// </summary>
        /// <param name="e">イベント引数</param>
        public void MouseUp(MouseEventArgs e)
        {
            // 左クリック押下時のみ選択ウィンドウハンドルを設定する
            e.Handled = true;
            IntPtr handle = IntPtr.Zero;
            if (e.LeftButton == MouseButtonState.Released)
            {
                handle = this._selectControlInfo.Handle;
            }
            SelectedControl(handle);
        }

        /// <summary>
        /// キーダウンイベント
        /// </summary>
        /// <param name="e"></param>
        public void KeyDown(KeyEventArgs e)
        {
            e.Handled = true;
            SelectedControl(IntPtr.Zero);
        }

        /// <summary>
        /// 選択中のコントロール情報の更新
        /// </summary>
        /// <param name="controlInfo">新しいコントロール情報</param>
        private void UpdateSelectControlInfo(ControlInfo controlInfo)
        {
            if (this._selectControlInfo != controlInfo)
            {
                // 選択コントロール情報の更新
                this._selectControlInfo = controlInfo;

                // コントロール選択情報ViewModelの更新
                ControlSelectInfo.ClassName = this._selectControlInfo.ClassName;
                ControlSelectInfo.Point = this._selectControlInfo.Bounds.Location;
                ControlSelectInfo.Size = this._selectControlInfo.Bounds.Size;

                // 選択範囲の設定メッセージ送信
                this.Messenger.Raise(new SetRectangleBoundsMessage
                {
                    MessageKey = "Rectangle.Bounds",
                    Left = this._selectControlInfo.Bounds.Left - this._screenLocation.X,
                    Top = this._selectControlInfo.Bounds.Top - this._screenLocation.Y,
                    Width = this._selectControlInfo.Bounds.Width,
                    Height = this._selectControlInfo.Bounds.Height
                });
            }
        }

        /// <summary>
        /// コントロールを選択します。
        /// </summary>
        /// <param name="handle">ハンドル</param>
        private void SelectedControl(IntPtr handle)
        {
            this.SelectedHandle = handle;
            this.Messenger.Raise(new InteractionMessage("Window.Close"));
        }
    }
}
