using System;
using System.Drawing;
using WinCap.Drivers;
using WinCap.Interop;
using WinCap.Serialization;
using WinCap.Services;

namespace WinCap.Capturers
{
    public class WebBrowserCapturer : CapturerBase<IntPtr?>
    {
        /// <summary>
        /// IEを表すクラス名
        /// </summary>
        protected const string ClassNameIe = "Internet Explorer_Server";

        /// <summary>
        /// ウィンドウサービス
        /// </summary>
        private readonly WindowService windowService;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="windowService">ウィンドウサービス</param>
        public WebBrowserCapturer(WindowService windowService)
        {
            this.windowService = windowService;
        }

        /// <summary>
        /// キャプチャのコア処理。
        /// </summary>
        /// <param name="target">キャプチャ対象</param>
        /// <returns>キャプチャ画像</returns>
        protected override Bitmap CaptureCore(IntPtr? target)
        {
            var handle = target.Value;

            // IE判定
            var className = InteropHelper.GetClassName(handle);
            if (className == ClassNameIe)
            {
                // ウェブページ全体をキャプチャ
                return InternetExplorerCapture(handle);
            }
            else
            {
                // 選択コントロールをキャプチャ
                return ScreenHelper.CaptureScreen(handle);
            }
        }

        /// <summary>
        /// キャプチャ対象を取得します。
        /// </summary>
        /// <returns>キャプチャ対象</returns>
        protected override IntPtr? GetTargetCore()
        {
            var handle = this.windowService.ShowControlSelectionWindow();
            return (handle != IntPtr.Zero
                ? handle as IntPtr?
                : null);
        }

        /// <summary>
        /// InternetExplorerのページ全体をキャプチャします。
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        /// <returns>ビットマップ</returns>
        private Bitmap InternetExplorerCapture(IntPtr handle)
        {
            var isScrollWindowPageTop = Settings.General.IsWebPageCaptureStartWhenPageFirstMove.Value;
            var scrollDelayTime = Settings.General.ScrollDelayTime.Value;

            using (InternetExplorer ie = new InternetExplorer(handle))
            {
                // DPI取得
                var dpi = PerMonitorDpi.GetDpi(handle);

                // クライアント領域、スクロール位置、スクロールサイズの取得
                Rectangle client = ie.Client;
                Point scrollPoint = ie.ScrollPoint;
                Size scrollSize = ie.ScrollSize;

                // ページ先頭移動フラグが設定されている場合、スクロール位置をページ先頭に設定する
                if (isScrollWindowPageTop)
                {
                    scrollPoint = ie.ScrollTo(0, 0);
                }

                // 出力先のビットマップ生成
                // クライアント領域の幅 x (スクロール領域の高さ - スクロール位置Y)
                Bitmap bmp = new Bitmap((int)((scrollSize.Width - scrollPoint.X) * dpi.ScaleX), (int)((scrollSize.Height - scrollPoint.Y) * dpi.ScaleY));

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    // スクロール開始/終端座標を求める
                    // ※終了座標はマージン前の高さで求める
                    Point scrollStart = new Point(scrollPoint.X, scrollPoint.Y);
                    Point scrollEnd = new Point(Math.Max(scrollSize.Width - client.Width, 0), Math.Max(scrollSize.Height - client.Height, 0));

                    // 右終端までスクロールしながらキャプチャする
                    CaptureControl(handle, g, ref client, ref scrollPoint, ref scrollStart, dpi);
                    while (scrollPoint.X < scrollEnd.X)
                    {
                        // スクロールしてキャプチャする
                        scrollPoint = ie.ScrollTo(scrollPoint.X + client.Width, scrollPoint.Y, scrollDelayTime);
                        CaptureControl(handle, g, ref client, ref scrollPoint, ref scrollStart, dpi);
                    }

                    // 終端までスクロールしながらキャプチャする
                    int prevScrollY = -1;
                    while (scrollPoint.Y < scrollEnd.Y && prevScrollY != scrollPoint.Y)
                    {
                        prevScrollY = scrollPoint.Y;

                        // スクロールしてキャプチャする
                        scrollPoint.X = scrollStart.X;
                        scrollPoint = ie.ScrollTo(scrollPoint.X, scrollPoint.Y + client.Height, scrollDelayTime);
                        CaptureControl(handle, g, ref client, ref scrollPoint, ref scrollStart, dpi);

                        // 右端までスクロールしながらキャプチャする
                        while (scrollPoint.X < scrollEnd.X)
                        {
                            // スクロールしてウィンドウキャプチャする
                            scrollPoint = ie.ScrollTo(scrollPoint.X + client.Width, scrollPoint.Y, scrollDelayTime);
                            CaptureControl(handle, g, ref client, ref scrollPoint, ref scrollStart, dpi);
                        }
                    }
                }
                return bmp;
            }
        }

        /// <summary>
        /// コントロールをキャプチャする
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        /// <param name="g">描画サーフェイス</param>
        /// <param name="client">クライアント領域</param>
        /// <param name="scrollPoint">スクロール座標</param>
        /// <param name="startScroll">開始スクロール座標</param>
        /// <param name="dpi">DPI情報</param>
        private void CaptureControl(IntPtr handle, Graphics g, ref Rectangle client, ref Point scrollPoint, ref Point scrollStart, Dpi dpi)
        {
            // ウィンドウをキャプチャする
            using (Bitmap bitmap = ScreenHelper.CaptureScreen(handle))
            {
                // 描画元領域の設定
                Rectangle srcRect = client.ToPhysicalPixel(dpi);

                // 描画先領域の設定
                Rectangle destRect = new Rectangle(
                    scrollPoint.X - scrollStart.X, scrollPoint.Y - scrollStart.Y, client.Width, client.Height
                ).ToPhysicalPixel(dpi);

                // キャプチャした画像を書き込む
                g.DrawImage(bitmap, destRect, srcRect, GraphicsUnit.Pixel);
            }
        }
    }
}
