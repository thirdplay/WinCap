using Livet;
using Livet.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WinCap.Models;
using WinCap.Utilities.Drawing;

namespace WinCap.ViewModels
{
    /// <summary>
    /// 選択ウィンドウVM
    /// </summary>
    public class SelectWindowViewModel : ViewModel
    {
        #region フィールド
        /// <summary>
        /// ウィンドウハンドルリスト
        /// </summary>
        protected List<IntPtr> handles_ = new List<IntPtr>();
        #endregion

        #region プロパティ
        /// <summary>
        /// 画面イメージ
        /// </summary>
        private BitmapSource _ScreenImage;
        public BitmapSource ScreenImage
        {
            get { return _ScreenImage; }
            set
            {
                if (this._ScreenImage != value)
                {
                    this._ScreenImage = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SelectWindowViewModel() { }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            // 画面全体をキャプチャする
            ScreenCapture capturer = new ScreenCapture();
            using (Bitmap bitmap = capturer.Capture())
            {
                _ScreenImage = bitmap.ToBitmapSource();
            }

            // 表示中のウィンドウハンドルを取得する
            //handles_ = WindowXxxx.GetWindowHandles();
        }

        private ListenerCommand<MouseEventArgs> _MouseMoveCommand;
        public ListenerCommand<MouseEventArgs> MouseMoveCommand
        {
            get
            {
                if (_MouseMoveCommand == null)
                {
                    _MouseMoveCommand = new ListenerCommand<MouseEventArgs>(MouseMove);
                }
                return _MouseMoveCommand;
            }
        }

        /// <summary>
        /// マウス移動
        /// </summary>
        /// <param name="e">イベント引数</param>
        private void MouseMove(MouseEventArgs e)
        {
            /*
            // 選択中のウィンドウ情報をクリアする
            //selectWindowInfo_.Clear();

            // 全てのウィンドウを調べる
            foreach (IntPtr handle in handles_)
            {
                // ハンドルの矩形情報の取得
                Rectangle rect = Win32.GetWindowRect(handle);
                if (rect != Rectangle.Empty)
                {
                    // ヒットチェック
                    if (Cursor.Position.X >= rect.Left && Cursor.Position.X <= rect.Right
                    && Cursor.Position.Y >= rect.Top && Cursor.Position.Y <= rect.Bottom) {
                        // ウィンドウクラス名の取得
                        string className = Win32.GetClassName(handle);

                        // 選択したウィンドウの情報を設定する
                        selectWindowInfo_.Handle = handle;
                        selectWindowInfo_.Rectangle = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                        selectWindowInfo_.ClassName = className.ToString();
                        break;
                    }
                }
            }
            */
        }
    }
}
