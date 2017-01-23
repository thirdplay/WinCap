using System;
using System.Drawing;
using WinCap.Drivers;

namespace WinCap.Capturers
{
    /// <summary>
    /// IEをキャプチャする機能を提供します。
    /// </summary>
    public class InternetExplorerCapturer : IWebBrowserCapturer
    {
        /// <summary>
        /// コントロールキャプチャ
        /// </summary>
        private readonly ControlCapturer capturer = new ControlCapturer();

        #region IWebBrowserCapturer members
        /// <summary>
        /// 自動スクロールの遅延時間
        /// </summary>
        public int ScrollDelayTime { get; set; }

        /// <summary>
        /// スクロールウィンドウ開始時のページ先頭移動フラグ
        /// </summary>
        public bool IsScrollWindowPageTop { get; set; }

        /// <summary>
        /// キャプチャ可能かどうか判定します。
        /// </summary>
        /// <param name="className">ウィンドウクラス名</param>
        /// <returns>判定結果</returns>
        public bool CanCapture(string className)
        {
            return className == "Internet Explorer_Server";
        }

        /// <summary>
        /// InternetExplorerのページ全体をキャプチャします。
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        /// <returns>ビットマップ</returns>
        public Bitmap Capture(IntPtr handle)
        {
            // IE操作クラスを生成する
            using (InternetExplorer ie = new InternetExplorer(handle))
            {
                // クライアント領域、スクロール位置、スクロールサイズの取得
                Rectangle client = ie.Client;
                Point scrollPoint = ie.ScrollPoint;
                Size scrollSize = ie.ScrollSize;

                // ページ先頭移動フラグが設定されている場合、スクロール位置をページ先頭に設定する
                if (this.IsScrollWindowPageTop)
                {
                    scrollPoint = ie.ScrollTo(0, 0);
                }

                // 出力先のビットマップ生成
                // クライアント領域の幅 x (スクロール領域の高さ - スクロール位置Y)
                Bitmap bmp = new Bitmap(scrollSize.Width - scrollPoint.X, scrollSize.Height - scrollPoint.Y);

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    // スクロール開始/終端座標を求める
                    // ※終了座標はマージン前の高さで求める
                    Point scrollStart = new Point(scrollPoint.X, scrollPoint.Y);
                    Point scrollEnd = new Point(Math.Max(scrollSize.Width - client.Width, 0), Math.Max(scrollSize.Height - client.Height, 0));

                    // 右終端までスクロールしながらキャプチャする
                    captureControl(handle, g, ref client, ref scrollPoint, ref scrollStart);
                    while (scrollPoint.X < scrollEnd.X)
                    {
                        // スクロールしてキャプチャする
                        scrollPoint = ie.ScrollTo(scrollPoint.X + client.Width, scrollPoint.Y, this.ScrollDelayTime);
                        captureControl(handle, g, ref client, ref scrollPoint, ref scrollStart);
                    }

                    // 終端までスクロールしながらキャプチャする
                    int prevScrollY = -1;
                    while (scrollPoint.Y < scrollEnd.Y && prevScrollY != scrollPoint.Y)
                    {
                        prevScrollY = scrollPoint.Y;

                        // スクロールしてキャプチャする
                        scrollPoint.X = scrollStart.X;
                        scrollPoint = ie.ScrollTo(scrollPoint.X, scrollPoint.Y + client.Height, this.ScrollDelayTime);
                        captureControl(handle, g, ref client, ref scrollPoint, ref scrollStart);

                        // 右端までスクロールしながらキャプチャする
                        while (scrollPoint.X < scrollEnd.X)
                        {
                            // スクロールしてウィンドウキャプチャする
                            scrollPoint = ie.ScrollTo(scrollPoint.X + client.Width, scrollPoint.Y, this.ScrollDelayTime);
                            captureControl(handle, g, ref client, ref scrollPoint, ref scrollStart);
                        }
                    }
                }
                return bmp;
            }
        }
        #endregion

        /// <summary>
        /// コントロールをキャプチャする
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="g">描画サーフェイス</param>
        /// <param name="client">クライアント領域</param>
        /// <param name="scrollPoint">スクロール座標</param>
        /// <param name="startScroll">開始スクロール座標</param>
        private void captureControl(IntPtr hWnd, Graphics g, ref Rectangle client, ref Point scrollPoint, ref Point scrollStart)
        {
            // 描画先領域の設定
            Rectangle destRect = new Rectangle(scrollPoint.X - scrollStart.X, scrollPoint.Y - scrollStart.Y, client.Width, client.Height);

            // ウィンドウをキャプチャする
            using (Bitmap bitmap = capturer.CaptureControl(hWnd))
            {
                // キャプチャした画像を書き込む
                g.DrawImage(bitmap, destRect, client, GraphicsUnit.Pixel);
            }
        }
    }
}
