using Livet;
using Livet.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WinCap.Models;
using WinCap.Utilities.Drawing;
using WinCap.ViewModels.Messages;

namespace WinCap.ViewModels
{
    /// <summary>
    /// 選択ウィンドウVM
    /// </summary>
    public class ControlSelectWindowViewModel : ViewModel
    {
        #region フィールド
        /// <summary>
        /// ウィンドウヘルパー
        /// </summary>
        private WindowHelper windowHelper = new WindowHelper();

        /// <summary>
        /// ウィンドウ情報リスト
        /// </summary>
        private List<WindowInfo> windowList = new List<WindowInfo>();
        #endregion

        #region プロパティ
        /// <summary>
        /// 選択ウィンドウ情報
        /// </summary>
        public WindowInfo SelectWindowInfo { get; set; }
        #endregion

        /// <summary>
		/// <see cref="Window.ContentRendered"/>イベントが発生したときに
        /// Livet インフラストラクチャによって呼び出されます。
        /// </summary>
        public void Initialize()
        {
            // 表示中のウィンドウ情報リストを取得する
            windowList = windowHelper.GetWindowList().Where(x => !x.Size.IsEmpty).ToList();

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
            this.SelectWindowInfo = null;
            foreach (WindowInfo windowInfo in windowList)
            {
                if (p.X >= windowInfo.Left && p.X <= windowInfo.Right
                && p.Y >= windowInfo.Top && p.Y <= windowInfo.Bottom)
                {
                    this.SelectWindowInfo = windowInfo;
                    this.Messenger.Raise(new SetRectangleBoundsMessage
                    {
                        MessageKey = "Rectangle.Bounds",
                        Bounds = this.SelectWindowInfo.Bounds
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
                this.SelectWindowInfo = null;
                this.Messenger.Raise(new InteractionMessage("Window.Close"));
            }
        }

        /// <summary>
        /// キーダウンイベント
        /// </summary>
        /// <param name="e"></param>
        public void KeyDown(KeyEventArgs e)
        {
            this.SelectWindowInfo = null;
            this.Messenger.Raise(new InteractionMessage("Window.Close"));
        }
    }
}
