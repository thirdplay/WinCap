using Livet;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using WinCap.Interop;
using WinCap.Models;
using WinCap.Services;
using WinCap.Util.Mvvm;
using WinCap.ViewModels.Messages;

namespace WinCap.ViewModels
{
    /// <summary>
    /// コントロール選択ウィンドウViewModel
    /// </summary>
    public class ControlSelectionWindowViewModel : ViewModel
    {
        #region フィールド
        /// <summary>
        /// ウィンドウハンドルリスト
        /// </summary>
        private List<IntPtr> handleList = new List<IntPtr>();

        /// <summary>
        /// スクリーンの座標
        /// </summary>
        private System.Drawing.Point screenLocation;

        /// <summary>
        /// 選択中のコントロール情報
        /// </summary>
        private ControlInfo selectControlInfo = ControlInfo.Empty;
        #endregion

        #region プロパティ
        /// <summary>
        /// コントロール選択情報ViewModel
        /// </summary>
        public ControlSelectionInfoViewModel ControlSelectInfo { get; set; }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ControlSelectionWindowViewModel()
        {
            ControlSelectInfo = new ControlSelectionInfoViewModel().AddTo(this);
        }

        /// <summary>
		/// <see cref="Window.ContentRendered"/>イベントが発生したときに
        /// Livet インフラストラクチャによって呼び出されます。
        /// </summary>
        public void Initialize()
        {
            // 表示中のウィンドウハンドルリストを取得する
            handleList = WindowHelper.GetHandles();

            // ウィンドウに画面全体の範囲を設定する
            System.Drawing.Rectangle screenRect = ScreenHelper.GetFullScreenBounds();
            this.Messenger.Raise(new SetWindowBoundsMessage
            {
                MessageKey = "Window.Bounds",
                Left = screenRect.Left,
                Top = screenRect.Top,
                Width = screenRect.Width,
                Height = screenRect.Height
            });
            screenLocation = screenRect.Location;

            this.ControlSelectInfo.Messenger.Raise(new SetMarginMessage
            {
                MessageKey = "ControlSelectionInfo.Margin",
                Left = 12.0,
                Top = 12.0
            });

            this.ControlSelectInfo.Messenger.Raise(new SetVisibilityMessage
            {
                MessageKey = "ControlSelectionInfo.Visibility",
                Visibility = Visibility.Visible
            });
        }

        /// <summary>
        /// マウス移動イベント
        /// </summary>
        /// <param name="e">イベント引数</param>
        public void MouseMove(MouseEventArgs e)
        {
            // スクリーン座標の取得
            Point p = e.GetPosition(null);
            p.X = p.X + screenLocation.X;
            p.Y = p.Y + screenLocation.Y;

            // マウスカーソル上にあるウィンドウの情報を更新する
            ControlInfo selectControlInfo = ControlInfo.Empty;
            foreach (IntPtr handle in handleList)
            {
                System.Drawing.Rectangle bounds = NativeMethods.GetWindowBounds(handle);
                if (bounds != System.Drawing.Rectangle.Empty)
                {
                    if (p.X >= bounds.Left && p.X <= bounds.Right
                    && p.Y >= bounds.Top && p.Y <= bounds.Bottom)
                    {
                        selectControlInfo = new ControlInfo(handle, NativeMethods.GetClassName(handle), bounds);
                        break;
                    }
                }
            }
            UpdateSelectControlInfo(selectControlInfo);
        }

        /// <summary>
        /// マウスアップイベント
        /// </summary>
        /// <remarks>左クリック押下時のみ選択ウィンドウハンドルを設定する。</remarks>
        /// <param name="e">イベント引数</param>
        public void MouseUp(MouseEventArgs e)
        {
            e.Handled = true;
            IntPtr handle = IntPtr.Zero;
            if (e.LeftButton == MouseButtonState.Released)
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
            e.Handled = true;
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
                    Left = selectControlInfo.Bounds.Left - screenLocation.X,
                    Top = selectControlInfo.Bounds.Top - screenLocation.Y,
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
            this.Messenger.Raise(new SelectedControlMessage
            {
                MessageKey = "Window.SelectedControl",
                Handle = handle
            });
        }
    }
}
