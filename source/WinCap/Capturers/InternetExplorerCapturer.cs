using System;
using System.Drawing;
using WinCap.WebBrowsers;

namespace WinCap.Capturers
{
    /// <summary>
    /// IEをキャプチャする機能を提供します。
    /// </summary>
    public class InternetExplorerCapturer : IWebBrowserCapturer
    {
        /// <summary>
        /// 画面キャプチャ
        /// </summary>
        private readonly ControlCapturer _capturer = new ControlCapturer();

        #region プロパティ
        /// <summary>
        /// 自動スクロールの遅延時間
        /// </summary>
        public int AutoScrollDelayTime { get; set; } = 100;

        /// <summary>
        /// スクロールウィンドウ開始時のページ先頭移動フラグ
        /// </summary>
        public bool IsScrollWindowPageTop { get; set; } = true;
        #endregion

        #region IWebBrowserCapturer members
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
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <returns>ビットマップ</returns>
        public Bitmap Capture(IntPtr hWnd)
        {
            // IE操作クラスを生成する
            using (InternetExplorer ie = new InternetExplorer(hWnd))
            {
                // クライアント領域、スクロール位置、スクロールサイズの取得
                Rectangle client = ie.Client;
                Point scrollPoint = ie.ScrollPoint;
                Size scrollSize = ie.ScrollSize;

                // ページ先頭移動フラグが設定されている場合
                if (IsScrollWindowPageTop)
                {
                    // スクロール位置をページ先頭に設定する
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

                    // ウィンドウキャプチャ
                    captureControl(hWnd, g, ref client, ref scrollPoint, ref scrollStart);
                    // 右終端までスクロールしながらキャプチャする
                    while (scrollPoint.X < scrollEnd.X)
                    {
                        // IEをスクロールする
                        scrollPoint = ie.ScrollTo(scrollPoint.X + client.Width, scrollPoint.Y, AutoScrollDelayTime);
                        // ウィンドウキャプチャ
                        captureControl(hWnd, g, ref client, ref scrollPoint, ref scrollStart);
                    }

                    // 終端までスクロールしながらキャプチャする
                    int prevScrollY = -1;
                    while (scrollPoint.Y < scrollEnd.Y && prevScrollY != scrollPoint.Y)
                    {
                        prevScrollY = scrollPoint.Y;

                        // IEをスクロールする
                        scrollPoint.X = scrollStart.X;
                        scrollPoint = ie.ScrollTo(scrollPoint.X, scrollPoint.Y + client.Height, AutoScrollDelayTime);
                        // ウィンドウキャプチャ
                        captureControl(hWnd, g, ref client, ref scrollPoint, ref scrollStart);

                        // 右端までスクロールしながらキャプチャする
                        while (scrollPoint.X < scrollEnd.X)
                        {
                            // IEをスクロールする
                            scrollPoint = ie.ScrollTo(scrollPoint.X + client.Width, scrollPoint.Y, AutoScrollDelayTime);
                            // ウィンドウキャプチャ
                            captureControl(hWnd, g, ref client, ref scrollPoint, ref scrollStart);
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
            using (Bitmap bitmap = _capturer.CaptureControl(hWnd))
            {
                // キャプチャした画像を書き込む
                g.DrawImage(bitmap, destRect, client, GraphicsUnit.Pixel);
            }
        }
    }
}
