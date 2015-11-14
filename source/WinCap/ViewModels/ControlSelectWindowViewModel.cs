using Livet;
using Livet.Messaging;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using WinCap.Models;
using WinCap.Utilities.Drawing;
using WinCap.Utilities.Mvvm;
using WinCap.ViewModels.Messages;

namespace WinCap.ViewModels
{
    /// <summary>
    /// コントロール選択ウィンドウViewModel
    /// </summary>
    public class ControlSelectWindowViewModel : ViewModel
    {
        #region フィールド
        /// <summary>
        /// コントロール情報リスト
        /// </summary>
        private List<WindowInfo> controlList = new List<WindowInfo>();
        #endregion

        /// <summary>
        /// コントロール選択情報ViewModel
        /// </summary>
        public ControlSelectInfoViewModel ControlSelectInfo { get; set; }

        #region SelectControlInfo 変更通知プロパティ
        private WindowInfo _SelectControlInfo;
        /// <summary>
        /// 選択コントロール情報
        /// </summary>
        public WindowInfo SelectControlInfo
        {
            get { return _SelectControlInfo; }
            set
            {
                if (_SelectControlInfo != value)
                {
                    _SelectControlInfo = value;
                    if (value != null)
                    {
                        ControlSelectInfo.ClassName = value.ClassName;
                        ControlSelectInfo.Bounds = value.Bounds;
                    }
                    else
                    {
                        ControlSelectInfo.ClassName = null;
                        ControlSelectInfo.Bounds = System.Drawing.Rectangle.Empty;
                    }
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ControlSelectWindowViewModel()
        {
            ControlSelectInfo = new ControlSelectInfoViewModel().AddTo(this);
        }

        /// <summary>
		/// <see cref="Window.ContentRendered"/>イベントが発生したときに
        /// Livet インフラストラクチャによって呼び出されます。
        /// </summary>
        public void Initialize()
        {
            // 表示中のウィンドウ情報リストを取得する
            controlList = WindowHelper.GetWindowList();

            // ウィンドウに画面全体の範囲を設定する
            System.Drawing.Rectangle rect = System.Windows.Forms.Screen.AllScreens.GetBounds();
            this.Messenger.Raise(new SetWindowBoundsMessage
            {
                MessageKey = "Window.Bounds",
                Left = rect.Left,
                Top = rect.Top,
                Width = rect.Width,
                Height = rect.Height
            });
        }

        /// <summary>
        /// マウス移動イベント
        /// </summary>
        /// <param name="e">イベント引数</param>
        public void MouseMove(MouseEventArgs e)
        {
            Point p = e.GetPosition(null);

            // マウスカーソル上にあるウィンドウの情報を更新する
            this.SelectControlInfo = null;
            foreach (WindowInfo controlInfo in controlList)
            {
                if (p.X >= controlInfo.Bounds.Left && p.X <= controlInfo.Bounds.Right
                && p.Y >= controlInfo.Bounds.Top && p.Y <= controlInfo.Bounds.Bottom)
                {
                    //Console.WriteLine($"[{controlInfo.Handle}]{controlInfo.ClassName}: ({controlInfo.Bounds.X}x{controlInfo.Bounds.Y}), {controlInfo.Bounds.Width}x{controlInfo.Bounds.Height}");
                    this.SelectControlInfo = controlInfo;
                    this.Messenger.Raise(new SetRectangleBoundsMessage
                    {
                        MessageKey = "Rectangle.Bounds",
                        Bounds = this.SelectControlInfo.Bounds
                    });
                    break;
                }
            }
        }

        /// <summary>
        /// マウスダウンイベント
        /// </summary>
        /// <param name="e">イベント引数</param>
        public void MouseDown(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // 選択イベント発火
                //this.Messenger.Raise(new SelectedMessage{ MessageKey = "Control.Selected" });
            }
            else
            {
                this.SelectControlInfo = null;
                this.Messenger.Raise(new InteractionMessage("Window.Close"));
            }
        }

        /// <summary>
        /// キーダウンイベント
        /// </summary>
        /// <param name="e"></param>
        public void KeyDown(KeyEventArgs e)
        {
            this.SelectControlInfo = null;
            this.Messenger.Raise(new InteractionMessage("Window.Close"));
        }
    }
}
