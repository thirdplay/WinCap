using System.Drawing;
using System.Windows.Forms;
using WinCap.Utilities.Drawing;

namespace WinCap.Models
{
    /// <summary>
    /// 画面全体をキャプチャするクラス。
    /// </summary>
    public class ScreenCapture
    {
        /// <summary>
        /// 画面全体をキャプチャする。
        /// </summary>
        /// <returns>ビットマップ</returns>
        public Bitmap Capture()
        {
            Rectangle scrRect = Screen.AllScreens.GetBounds();
            return Capture(scrRect.X, scrRect.Y, scrRect.Width, scrRect.Height);
        }

        /// <summary>
        /// 指定範囲の画面をキャプチャする。
        /// </summary>
        /// <param name="x">左上のX座標</param>
        /// <param name="y">左上のY座標</param>
        /// <param name="width">横幅</param>
        /// <param name="height">高さ</param>
        /// <returns>ビットマップ</returns>
        public Bitmap Capture(int x, int y, int width, int height)
        {
            // 返却用のビットマップ生成
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // 黒でクリアする
                g.Clear(Color.Black);

                // 全てのスクリーンをコピーする
                foreach (Screen screen in Screen.AllScreens)
                {
                    Rectangle rect = screen.Bounds;
                    g.CopyFromScreen(rect.X, rect.Y, rect.X - x, rect.Y - y, rect.Size);
                }
            }

            return bmp;
        }
    }
}
