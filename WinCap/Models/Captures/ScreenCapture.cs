using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinCap.Models.Captures
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
            // 画面全体の解像度を求める
            Rectangle scrRect = new Rectangle();
            foreach (Screen screen in Screen.AllScreens)
            {
                scrRect.X = Math.Min(scrRect.X, screen.Bounds.Left);
                scrRect.Y = Math.Min(scrRect.Y, screen.Bounds.Top);
                scrRect.Width = Math.Max(scrRect.Width, screen.Bounds.Right);
                scrRect.Height = Math.Max(scrRect.Height, screen.Bounds.Bottom);
            }

            // オフセット分のサイズ補正
            scrRect.Width -= scrRect.X;
            scrRect.Height -= scrRect.Y;

            // 返却用のビットマップ生成
            Bitmap bmp = new Bitmap(scrRect.Width, scrRect.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // 黒でクリアする
                g.Clear(Color.Black);

                // 全てのスクリーンをコピーする
                foreach (Screen screen in Screen.AllScreens)
                {
                    Rectangle rect = screen.Bounds;
                    g.CopyFromScreen(rect.X, rect.Y, rect.X - scrRect.X, rect.Y - scrRect.Y, rect.Size);
                }
            }

            return bmp;
        }
    }
}
