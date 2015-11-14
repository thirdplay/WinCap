using Livet;
using Livet.Messaging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
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
        /// ウィンドウハンドルリスト
        /// </summary>
        private List<IntPtr> handleList = new List<IntPtr>();

        /// <summary>
        /// 選択中のコントロール情報
        /// </summary>
        private ControlInfo selectControlInfo = ControlInfo.Empty;
        #endregion

        #region プロパティ
        /// <summary>
        /// コントロール選択情報ViewModel
        /// </summary>
        public ControlSelectInfoViewModel ControlSelectInfo { get; set; }
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
            // 表示中のウィンドウハンドルリストを取得する
            handleList = WindowHelper.GetHandleList();

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

            this.ControlSelectInfo.Messenger.Raise(new SetMarginMessage
            {
                MessageKey = "ControlSelectInfo.Margin",
                Left = 12.0,
                Top = 12.0
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
            ControlInfo selectControlInfo = ControlInfo.Empty;
            foreach (IntPtr handle in handleList)
            {
                System.Drawing.Rectangle bounds = WindowHelper.GetWindowBounds(handle);
                if (bounds != System.Drawing.Rectangle.Empty)
                {
                    if (p.X >= bounds.Left && p.X <= bounds.Right
                    && p.Y >= bounds.Top && p.Y <= bounds.Bottom)
                    {
                        selectControlInfo = new ControlInfo(handle, WindowHelper.GetClassName(handle), bounds);
                        break;
                    }
                }
            }
            UpdateSelectControlInfo(selectControlInfo);
        }

        /// <summary>
        /// マウスダウンイベント
        /// </summary>
        /// <remarks>左クリック押下時のみ選択ウィンドウハンドルを設定する。</remarks>
        /// <param name="e">イベント引数</param>
        public void MouseDown(MouseEventArgs e)
        {
            IntPtr handle = IntPtr.Zero;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                handle = this.selectControlInfo.Handle;
            }
            executeSelect(handle);
        }

        /// <summary>
        /// キーダウンイベント
        /// </summary>
        /// <param name="e"></param>
        public void KeyDown(KeyEventArgs e)
        {
            executeSelect(IntPtr.Zero);
        }

        /// <summary>
        /// 選択中のコントロール情報の更新
        /// </summary>
        /// <param name="controlInfo">新しいコントロール情報</param>
        private void UpdateSelectControlInfo(ControlInfo controlInfo)
        {
            if (this.selectControlInfo != controlInfo)
            {
                // 選択コントロール情報の更新
                this.selectControlInfo = controlInfo;

                // コントロール選択情報ViewModelの更新
                ControlSelectInfo.ClassName = selectControlInfo.ClassName;
                ControlSelectInfo.Bounds = selectControlInfo.Bounds;

                // 選択範囲の設定メッセージ送信
                this.Messenger.Raise(new SetRectangleBoundsMessage
                {
                    MessageKey = "Rectangle.Bounds",
                    Left = selectControlInfo.Bounds.Left,
                    Top = selectControlInfo.Bounds.Top,
                    Width = selectControlInfo.Bounds.Width,
                    Height = selectControlInfo.Bounds.Height
                });
            }
        }

        /// <summary>
        /// コントロールを選択する。
        /// </summary>
        /// <param name="handle">ハンドル</param>
        private void executeSelect(IntPtr handle)
        {
            this.Messenger.Raise(new SelectControlMessage
            {
                MessageKey = "Window.SelectControl",
                Handle = handle
            });
            this.Messenger.Raise(new InteractionMessage("Window.Close"));
        }
    }
}
