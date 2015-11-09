using Livet;
using Livet.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
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

        /// <summary>
        /// マウス移動コマンド
        /// </summary>
        private ListenerCommand<Point> _MouseMoveCommand;
        public ListenerCommand<Point> MouseMoveCommand
        {
            get
            {
                if (_MouseMoveCommand == null)
                {
                    _MouseMoveCommand = new ListenerCommand<Point>(p =>
                    {
                        System.Console.WriteLine($"({p.X},{p.Y})");

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
                    });
                }
                return _MouseMoveCommand;
            }
        }
    }
}
