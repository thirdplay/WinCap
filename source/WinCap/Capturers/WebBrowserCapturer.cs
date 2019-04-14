using System;
using System.Diagnostics;
using System.Drawing;
using WinCap.Browsers;
using WinCap.Interop;
using WinCap.Serialization;
using WinCap.Services;

namespace WinCap.Capturers
{
    /// <summary>
    /// ウェブブラウザをキャプチャする機能を提供します。
    /// </summary>
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

        #region CapturerBase members 

        /// <summary>
        /// キャプチャ対象を取得します。
        /// </summary>
        /// <returns>キャプチャ対象</returns>
        protected override IntPtr? GetCaptureTarget()
        {
            var handle = this.windowService.ShowControlSelectionWindow();
            return (handle != IntPtr.Zero
                ? handle as IntPtr?
                : null);
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

        #endregion

        /// <summary>
        /// InternetExplorerのページ全体をキャプチャします。
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        /// <returns>ビットマップ</returns>
        private Bitmap InternetExplorerCapture(IntPtr handle)
        {
            var isScrollWindowPageTop = Settings.General.IsWebPageCaptureStartWhenPageFirstMove.Value;
            var scrollDelayTime = Settings.General.ScrollDelayTime.Value;
            var fixHeaderHeight = Settings.General.FixHeaderHeight.Value;

            using (InternetExplorer ie = new InternetExplorer(handle, scrollDelayTime))
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
                var bmpWidth = (int)((scrollSize.Width - scrollPoint.X) * dpi.ScaleX);
                var bmpHeight = (int)((scrollSize.Height - scrollPoint.Y) * dpi.ScaleY);
                Bitmap bmp = new Bitmap(bmpWidth, bmpHeight - (bmpHeight / client.Height - 1) * fixHeaderHeight);

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
                        scrollPoint = ie.ScrollTo(scrollPoint.X + client.Width, scrollPoint.Y);
                        CaptureControl(handle, g, ref client, ref scrollPoint, ref scrollStart, dpi);
                    }

                    // 先頭行以降は固定ヘッダーを除外してキャプチャする
                    client.Y += fixHeaderHeight;
                    client.Height -= fixHeaderHeight;

                    // 終端までスクロールしながらキャプチャする
                    bool isFirst = true;
                    int prevScrollY = -1;
                    while (scrollPoint.Y < scrollEnd.Y && prevScrollY != scrollPoint.Y)
                    {
                        prevScrollY = scrollPoint.Y;

                        // スクロールしてキャプチャする
                        scrollPoint.X = scrollStart.X;
                        scrollPoint = ie.ScrollTo(scrollPoint.X, scrollPoint.Y + client.Height);
                        CaptureControl(handle, g, ref client, ref scrollPoint, ref scrollStart, dpi, (isFirst ? fixHeaderHeight : 0));
                        isFirst = false;

                        // 右端までスクロールしながらキャプチャする
                        while (scrollPoint.X < scrollEnd.X)
                        {
                            // スクロールしてウィンドウキャプチャする
                            scrollPoint = ie.ScrollTo(scrollPoint.X + client.Width, scrollPoint.Y);
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
        /// <param name="fixHeaderHeight">固定ヘッダーの高さ</param>
        private void CaptureControl(IntPtr handle, Graphics g, ref Rectangle client, ref Point scrollPoint, ref Point scrollStart, Dpi dpi, int fixHeaderHeight = 0)
        {
            // ウィンドウをキャプチャする
            using (Bitmap bitmap = ScreenHelper.CaptureScreen(handle))
            {
                // 描画元領域の設定
                Rectangle srcRect = client.ToPhysicalPixel(dpi);

                // 描画先領域の設定
                Rectangle destRect = new Rectangle(
                    scrollPoint.X - scrollStart.X, scrollPoint.Y - scrollStart.Y + fixHeaderHeight, client.Width, client.Height
                ).ToPhysicalPixel(dpi);

                // キャプチャした画像を書き込む
                g.DrawImage(bitmap, destRect, srcRect, GraphicsUnit.Pixel);
            }
        }
    }
}
