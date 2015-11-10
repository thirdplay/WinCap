using Livet;
using Livet.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ControlSelectWindowViewModel() { }

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

        #region マウス移動コマンド
        /// <summary>
        /// マウス移動コマンド
        /// </summary>
        private ListenerCommand<MouseEventArgs> _MouseMoveCommand;
        public ListenerCommand<MouseEventArgs> MouseMoveCommand
        {
            get
            {
                if (_MouseMoveCommand == null)
                {
                    _MouseMoveCommand = new ListenerCommand<MouseEventArgs>(MouveMove);
                }
                return _MouseMoveCommand;
            }
        }

        /// <summary>
        /// マウス移動イベント
        /// </summary>
        /// <param name="e">イベント引数</param>
        private void MouveMove(MouseEventArgs e)
        {
            Point p = e.GetPosition(null);
            Console.WriteLine($"({p.X},{p.Y})");

            //Cursor.Position.X
            // 選択中のウィンドウ情報をクリアする
            //selectWindowInfo = null;

            /*
            // 全てのウィンドウを調べる
            foreach (WindowInfo windowInfo in windowList)
            {
                // ヒットチェック
                if (Cursor.Position.X >= windowInfo.Left && Cursor.Position.X <= windowInfo.Right
                && Cursor.Position.Y >= windowInfo.Top && Cursor.Position.Y <= windowInfo.Bottom) {
                    // 選択したウィンドウの情報を設定する
                    selectWindowInfo = windowInfo;
                    break;
                }
            }
            */
        }
        #endregion

        #region マウスダウンコマンド
        /// <summary>
        /// マウスダウンコマンド
        /// </summary>
        private ListenerCommand<MouseEventArgs> _MouseDownCommand;
        public ListenerCommand<MouseEventArgs> MouseDownCommand
        {
            get
            {
                if (_MouseDownCommand == null)
                {
                    _MouseDownCommand = new ListenerCommand<MouseEventArgs>(MouseDown);
                }
                return _MouseDownCommand;
            }
        }

        /// <summary>
        /// マウスダウンイベント
        /// </summary>
        /// <param name="e">イベント引数</param>
        private void MouseDown(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // 選択イベント発火
                //this.Messenger.Raise(new SelectedMessage{ MessageKey = "Control.Selected" });
            }
            else
            {
                //this.Messenger.Raise(new CloseWindowMessage);
            }
        }
        #endregion

        #region キーダウンコマンド
        /// <summary>
        /// キーダウンコマンド
        /// </summary>
        private ListenerCommand<KeyEventArgs> _KeyDownCommand;
        public ListenerCommand<KeyEventArgs> KeyDownCommand
        {
            get
            {
                if (_KeyDownCommand == null)
                {
                    _KeyDownCommand = new ListenerCommand<KeyEventArgs>(KeyDown);
                }
                return _KeyDownCommand;
            }
        }

        /// <summary>
        /// キーダウンイベント
        /// </summary>
        /// <param name="e"></param>
        private void KeyDown(KeyEventArgs e)
        {
            //this.Messenger.Raise(new CloseWindowMessage);
            Console.WriteLine(e.Key.ToString());
        }
        #endregion
    }
}
