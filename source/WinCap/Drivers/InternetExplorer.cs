using Microsoft.Win32;
using mshtml;
using SHDocVw;
using System;
using System.Drawing;
using System.Reactive.Disposables;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using WinCap.Interop;
using WinCap.Interop.Mshtml;
using WinCap.Interop.Win32;

namespace WinCap.Drivers
{
    /// <summary>
    /// IEの操作機能を提供します。
    /// </summary>
    public class InternetExplorer : IDisposable
    {
        #region 定数
        
        /// <summary>
        /// HTMLObject取得メッセージ
        /// </summary>
        private const string HtmlGetObjectMessage = "WM_HTML_GETOBJECT";

        /// <summary>
        /// 文字サイズ：中
        /// </summary>
        private const int CharSizeMiddle = 2;

        /// <summary>
        /// ズーム：等倍
        /// </summary>
        private const int ZoomActual = 100;

        #endregion

        /// <summary>
        /// リソース解放フラグ
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// ウィンドウハンドル
        /// </summary>
        private IntPtr handle = IntPtr.Zero;

        /// <summary>
        /// ウェブブラウザ情報
        /// </summary>
        private IWebBrowser2 webBrowser = null;

        /// <summary>
        /// HTMLウィンドウ
        /// </summary>
        private IHTMLWindow2 window = null;

        /// <summary>
        /// HTML文書本体
        /// </summary>
        private IHTMLElement2 body = null;

        /// <summary>
        /// 元の文字サイズ
        /// </summary>
        private int charSizeOriginal = CharSizeMiddle;

        /// <summary>
        /// 元の光学ズーム倍率
        /// </summary>
        private int zoomOriginal = ZoomActual;

        /// <summary>
        /// クライアント領域の矩形
        /// </summary>
        public Rectangle Client { get; set; }

        /// <summary>
        /// クライアント領域のオフセット
        /// </summary>
        public Point Offset { get; set; }

        /// <summary>
        /// スクロール位置
        /// </summary>
        public Point ScrollPoint => new Point(this.body.scrollLeft, this.body.scrollTop);

        /// <summary>
        /// スクロールサイズ
        /// </summary>
        public Size ScrollSize => new Size(this.body.scrollWidth, this.body.scrollHeight);

        /// <summary>
        /// 横スクロールバーが表示されているかどうか
        /// </summary>
        public bool IsVisibleScrollbarH => this.Client.Width < this.body.scrollWidth;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InternetExplorer()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        public InternetExplorer(IntPtr hWnd)
        {
            SetHandle(hWnd);
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~InternetExplorer()
        {
            Dispose(false);
        }

        /// <summary>
        /// 指定ウィンドウハンドルのIEを制御する
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        public void SetHandle(IntPtr handle)
        {
            IHTMLDocument2 document2 = null;
            IHTMLDocument3 document3 = null;
            IHTMLDocument6 document6 = null;
            try
            {
                // ウェブブラウザ情報を取得する
                this.webBrowser = GetWebBrowser(handle);
                if (this.webBrowser == null)
                {
                    // HTMLDocument情報が取得できなかった
                    throw new Exception("ウェブブラウザ情報の取得に失敗");
                }

                // IEのHTML文書を取得する
                document2 = (IHTMLDocument2)this.webBrowser.Document;
                document3 = (IHTMLDocument3)this.webBrowser.Document;
                document6 = (IHTMLDocument6)this.webBrowser.Document;

                // HTMLウィンドウと文書本体を取得
                this.window = document2.parentWindow;
                this.body = (IHTMLElement2)document3.documentElement;
                if (this.body.clientWidth == 0 && this.body.clientHeight == 0)
                {
                    // クライアントサイズが0の場合はbodyを参照
                    this.ReleaseComObject(this.body);
                    this.body = (IHTMLElement2)document2.body;
                }

                // クライアント領域のオフセットを取得
                this.Offset = GetMargin(document6.documentMode.ToString());
                this.Client = new Rectangle(this.Offset.X, this.Offset.Y, this.body.clientWidth, this.body.clientHeight);

                // 横スクロールバーが表示中かつ
                // 「ウィンドウ高さ」と「クライアント高さ」の差分が「水平スクロールバーの高さ」より小さい場合
                Rectangle wndRect = InteropHelper.GetWindowSize(handle);
                if (this.IsVisibleScrollbarH && (wndRect.Height - this.Client.Height) < SystemInformation.HorizontalScrollBarHeight)
                {
                    // ※暫定的にクライアント高さからスクロールバー分を引いて処理する
                    this.Client = new Rectangle(this.Client.X, this.Client.Y, this.Client.Width, this.Client.Height - SystemInformation.HorizontalScrollBarHeight);
                }


                // 文字サイズと光学ズームの倍率取得
                this.charSizeOriginal = GetCharSize();
                this.zoomOriginal = GetZoom();

                // 文字サイズとズームを等倍にする
                this.SetCharSize(CharSizeMiddle);
                this.SetZoom(ZoomActual);

                // 最後にウィンドウハンドルを登録する
                this.handle = handle;
            }
            finally
            {
                // COMオブジェクトの解放
                ReleaseComObject(document2);
                ReleaseComObject(document3);
                ReleaseComObject(document6);
            }
        }

        /// <summary>
        /// 倍率を返す
        /// </summary>
        /// <returns>倍率</returns>
        public int GetCharSize()
        {
            return (int)ExecWB(OLECMDID.OLECMDID_ZOOM, OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER);
        }

        /// <summary>
        /// 文字サイズを設定する
        /// </summary>
        /// <param name="size">倍率[0-4]</param>
        public void SetCharSize(int size)
        {
            if (GetCharSize() != size)
            {
                ExecWB(OLECMDID.OLECMDID_ZOOM, OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER, size);
            }
        }

        /// <summary>
        /// 光学ズームの倍率を返す
        /// </summary>
        /// <returns>倍率</returns>
        public int GetZoom()
        {
            const string key = @"Software\Microsoft\Internet Explorer\Zoom";
            using (RegistryKey regkey = Registry.CurrentUser.OpenSubKey(key, false))
            {
                return (int)regkey.GetValue("ZoomFactor", 100000) / 1000;
            }
        }

        /// <summary>
        /// 光学ズームの倍率を設定する
        /// </summary>
        /// <param name="zoom">倍率[10-1000]</param>
        public void SetZoom(int zoom)
        {
            if (GetZoom() != zoom)
            {
                ExecWB(OLECMDID.OLECMDID_OPTICAL_ZOOM, OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER, zoom);
            }
        }

        /// <summary>
        /// スクロール位置を設定する
        /// </summary>
        /// <param name="x">スクロール位置X</param>
        /// <param name="y">スクロール位置Y</param>
        /// <param name="delayTime">遅延時間</param>
        /// <returns>設定後のスクロール位置</returns>
        public Point ScrollTo(int x, int y)
        {
            return ScrollTo(x, y, 0);
        }

        /// <summary>
        /// スクロール位置を設定する
        /// </summary>
        /// <param name="x">スクロール位置X</param>
        /// <param name="y">スクロール位置Y</param>
        /// <param name="delayTime">遅延時間</param>
        /// <returns>設定後のスクロール位置</returns>
        public Point ScrollTo(int x, int y, int delayTime)
        {
            // スクロール位置の設定
            Point nextPoint = new Point(Math.Min(x, this.ScrollSize.Width - this.Client.Width), Math.Min(y, this.ScrollSize.Height - this.Client.Height));
            this.window.scrollTo(nextPoint.X, nextPoint.Y);

            // 遅延時間が設定されている場合、指定時間スリープする
            if (delayTime > 0)
            {
                Thread.Sleep(delayTime);
            }

            // スクロール位置が指定スクロール位置より小さい場合、指定スクロール位置に補正する
            Point p = this.ScrollPoint;
            //if (p.X < nextPoint.Y) { p.X = nextPoint.X; }
            if (p.Y < nextPoint.Y) { p.Y = nextPoint.Y; }
            return p;
        }

        /// <summary>
        /// 指定ウィンドウのウェブブラウザ情報を取得する
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <returns>ウェブブラウザ情報</returns>
        private IWebBrowser2 GetWebBrowser(IntPtr hWnd)
        {
            UIntPtr sendMessageResult = UIntPtr.Zero;
            Guid IID_IHTMLDocument3 = typeof(IHTMLDocument3).GUID;
            IWebBrowser2 wb = null;

            // WM_HTML_GETOBJECTメッセージの登録
            uint msg = User32.RegisterWindowMessage(HtmlGetObjectMessage);

            // WM_HTML_GETOBJECTメッセージの送信
            User32.SendMessageTimeout(hWnd, msg, 0, 0, (uint)SMTO.ABORTIFHUNG, 1000, ref sendMessageResult);
            if (sendMessageResult == UIntPtr.Zero)
            {
                return null;
            }

            // HTML文書情報の取得
            IHTMLDocument2 doc = null;
            if (Oleacc.ObjectFromLresult(sendMessageResult, ref IID_IHTMLDocument3, 0, ref doc) != 0)
            {
                return null;
            }
            using (Disposable.Create(() => ReleaseComObject(doc)))
            {
                // ウェブブラウザの取得
                var serviceProvider = (Interop.Win32.IServiceProvider)doc.parentWindow;
                using (Disposable.Create(() => ReleaseComObject(serviceProvider)))
                {
                    Guid webBrowserAppGUID = typeof(IWebBrowserApp).GUID;
                    Guid webBrowserGUID = typeof(IWebBrowser2).GUID;
                    object result;
                    serviceProvider.QueryService(webBrowserAppGUID, webBrowserGUID, out result);
                    wb = (IWebBrowser2)result;

                }
            }
            return wb;
        }

        /// <summary>
        /// クライアント領域のマージンを取得する
        /// </summary>
        /// <param name="documentMode">ドキュメントモード</param>
        /// <returns>クライアント領域のマージン</returns>
        private Point GetMargin(string documentMode)
        {
            Point rect = Point.Empty;
            if (documentMode == "7" || documentMode == "8")
            {
                rect = new Point(2, 2);
            }
            return rect;
        }

        /// <summary>
        /// ウェブブラウザの取得操作の実行
        /// </summary>
        /// <param name="cmdId">コマンドID</param>
        /// <param name="cmdExecOpt">コマンド実行オプション</param>
        /// <returns>結果</returns>
        private object ExecWB(OLECMDID cmdId, OLECMDEXECOPT cmdExecOpt)
        {
            object pvaIn = Type.Missing;
            object pvaOut = Type.Missing;
            this.webBrowser.ExecWB(cmdId, cmdExecOpt, ref pvaIn, ref pvaOut);
            return pvaOut;
        }

        /// <summary>
        /// ウェブブラウザの設定操作の実行
        /// </summary>
        /// <param name="cmdId">コマンドID</param>
        /// <param name="cmdExecOpt">コマンド実行オプション</param>
        /// <param name="param">設定パラメータ</param>
        private void ExecWB(OLECMDID cmdId, OLECMDEXECOPT cmdExecOpt, object param)
        {
            object pvaIn = param;
            object pvaOut = Type.Missing;
            this.webBrowser.ExecWB(cmdId, cmdExecOpt, ref pvaIn, ref pvaOut);
        }

        /// <summary>
        /// COMオブジェクトを解放する
        /// </summary>
        /// <param name="obj">COMオブジェクト</param>
        private void ReleaseComObject(object obj)
        {
            // COMオブジェクト以外は処理しない
            if (obj == null) { return; }
            if (!Marshal.IsComObject(obj)) { return; }

            // COMオブジェクトの解放
            Marshal.FinalReleaseComObject(obj);
        }

        #region IDisposable members

        /// <summary>
        /// リソース解放
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // ガベコレからデストラクタを対象外にする
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// リソース解放
        /// </summary>
        /// <param name="disposing">リソース解放中フラグ</param>
        protected virtual void Dispose(bool disposing)
        {
            lock (this)
            {
                if (this.disposed)
                {
                    return;
                }

                this.disposed = true;

                if (disposing)
                {
                    // マネージリソースの解放
                }

                // ウィンドウハンドルが登録されている場合
                if (this.handle != IntPtr.Zero)
                {
                    // 文字サイズとズームを復元する
                    SetCharSize(this.charSizeOriginal);
                    SetZoom(this.zoomOriginal);
                }

                // アンマネージリソースの解放
                ReleaseComObject(this.body);
                ReleaseComObject(this.window);
                ReleaseComObject(this.webBrowser);
            }
        }

        #endregion
    }
}
