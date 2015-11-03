using Livet;
using System.Drawing;
using System.Windows.Media.Imaging;
using WinCap.Models.Captures;
using WinCap.Utilities.Drawing;
using WinCap.Utilities.Mvvm;
using WinCap.Win32;

namespace WinCap.ViewModels
{
    /// <summary>
    /// 選択ウィンドウVM
    /// </summary>
    public class SelectWindowViewModel : ViewModel
    {
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
        }
    }
}
