using Livet;
using Livet.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
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
            using (System.Drawing.Bitmap bitmap = capturer.Capture())
            {
                _ScreenImage = bitmap.ToBitmapSource();
            }

            // 表示中のウィンドウ情報リストを取得する
            windowList = windowHelper.GetWindowList().Where(x => !x.Size.IsEmpty).ToList();
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
