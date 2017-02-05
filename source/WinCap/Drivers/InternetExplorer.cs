using Microsoft.Win32;
using MSHTML;
using SHDocVw;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using WinCap.Interop;
using WinCap.Interop.Win32;
using WinCap.Models;

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
        protected static readonly string HtmlGetObjectMessage = "WM_HTML_GETOBJECT";

        /// <summary>
        /// 文字サイズ：中
        /// </summary>
        protected static readonly int CharSizeMiddle = 2;

        /// <summary>
        /// ズーム：等倍
        /// </summary>
        protected static readonly int ZoomActual = 100;

        #endregion

        #region フィールド

        /// <summary>
        /// リソース解放フラグ
        /// </summary>
        protected bool disposed = false;

        /// <summary>
        /// ウィンドウハンドル
        /// </summary>
        protected IntPtr handle = IntPtr.Zero;

        /// <summary>
        /// ウェブブラウザ情報
        /// </summary>
        protected IWebBrowser2 webBrowser = null;

        /// <summary>
        /// HTML文書2
        /// </summary>
        protected IHTMLDocument2 document2 = null;

        /// <summary>
        /// HTML文書3
        /// </summary>
        protected IHTMLDocument3 document3 = null;

        /// <summary>
        /// HTML文書6
        /// </summary>
        protected IHTMLDocument6 document6 = null;

        /// <summary>
        /// HTMLウィンドウ
        /// </summary>
        protected IHTMLWindow2 window = null;

        /// <summary>
        /// HTML文書本体
        /// </summary>
        protected IHTMLElement2 body = null;

        /// <summary>
        /// ドキュメントモード
        /// </summary>
        protected string documentMode = null;

        /// <summary>
        /// 元の文字サイズ
        /// </summary>
        protected int charSizeOriginal = CharSizeMiddle;

        /// <summary>
        /// 元の光学ズーム倍率
        /// </summary>
        protected int zoomOriginal = ZoomActual;

        #endregion

        #region プロパティ
        
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
        public Point ScrollPoint => new Point(body.scrollLeft, body.scrollTop);

        /// <summary>
        /// スクロールサイズ
        /// </summary>
        public Size ScrollSize => new Size(body.scrollWidth, body.scrollHeight);

        /// <summary>
        /// 横スクロールバーが表示されているかどうか
        /// </summary>
        public bool IsVisibleScrollbarH => Client.Width < body.scrollWidth;

        #endregion

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
            // ウェブブラウザ情報を取得する
            this.webBrowser = GetWebBrowser(handle);
            if (this.webBrowser == null)
            {
                // HTMLDocument情報が取得できなかった
                throw new Exception("ウェブブラウザ情報の取得に失敗");
            }

            // IEのHTML文書を取得する
            this.document2 = (IHTMLDocument2)this.webBrowser.Document;
            this.document3 = (IHTMLDocument3)this.webBrowser.Document;
            this.document6 = (IHTMLDocument6)this.webBrowser.Document;

            // HTMLウィンドウを取得する
            this.window = this.document2.parentWindow;

            // HTML文書本体の取得
            this.body = (IHTMLElement2)this.document3.documentElement;
            // クライアントサイズが0の場合はbodyを参照する
            if (this.body.clientWidth == 0 && this.body.clientHeight == 0)
            {
                this.ReleaseComObject(body);
                this.body = (IHTMLElement2)this.document2.body;
            }

            // ドキュメントモードの取得
            this.documentMode = this.document6.documentMode.ToString();

            // クライアント領域のオフセットを取得
            this.Offset = GetMargin(this.documentMode);
            var rect = new Rectangle();

            // 高DPI対応
            var dpi = PerMonitorDpi.GetDpi(handle);
            rect.X = (int)(this.Offset.X * (1 / dpi.ScaleX));
            rect.Y = (int)(this.Offset.Y * (1 / dpi.ScaleY));
            rect.Width = (int)(this.body.clientWidth * (1 / dpi.ScaleX));
            rect.Height = (int)(this.body.clientHeight * (1 / dpi.ScaleY));
            this.Client = rect;

            // 横スクロールバーが表示中かつ
            // 「ウィンドウ高さ」と「クライアント高さ」の差分が「水平スクロールバーの高さ」より小さい場合
            Rectangle wndRect = InteropExtensions.GetWindowBounds(handle);
            if (IsVisibleScrollbarH && (wndRect.Height - Client.Height) < SystemInformation.HorizontalScrollBarHeight)
            {
                // ※暫定的にクライアント高さからスクロールバー分を引いて処理する
                this.Client = new Rectangle(Client.X, Client.Y, Client.Width, Client.Height - SystemInformation.HorizontalScrollBarHeight);
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
            using (RegistryKey regkey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\Zoom", false))
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
            Point nextPoint = new Point(Math.Min(x, ScrollSize.Width - Client.Width), Math.Min(y, ScrollSize.Height - Client.Height));
            window.scrollTo(nextPoint.X, nextPoint.Y);

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
        protected IWebBrowser2 GetWebBrowser(IntPtr hWnd)
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

            IHTMLDocument2 doc = null;
            try
            {
                // HTML文書情報の取得
                if (Oleacc.ObjectFromLresult(sendMessageResult, ref IID_IHTMLDocument3, 0, ref doc) != 0)
                {
                    return null;
                }

                // ウェブブラウザの取得
                var serviceProvider = (Interop.Win32.IServiceProvider)doc.parentWindow;
                try
                {
                    Guid webBrowserAppGUID = typeof(IWebBrowserApp).GUID;
                    Guid webBrowserGUID = typeof(IWebBrowser2).GUID;
                    object result;
                    serviceProvider.QueryService(webBrowserAppGUID, webBrowserGUID, out result);
                    wb = (IWebBrowser2)result;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    ReleaseComObject(serviceProvider);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                ReleaseComObject(doc);
            }
            return wb;
        }

        /// <summary>
        /// クライアント領域のマージンを取得する
        /// </summary>
        /// <param name="documentMode">ドキュメントモード</param>
        /// <returns>クライアント領域のマージン</returns>
        protected Point GetMargin(string documentMode)
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
        protected object ExecWB(OLECMDID cmdId, OLECMDEXECOPT cmdExecOpt)
        {
            object pvaIn = Type.Missing;
            object pvaOut = Type.Missing;
            webBrowser.ExecWB(cmdId, cmdExecOpt, ref pvaIn, ref pvaOut);
            return pvaOut;
        }

        /// <summary>
        /// ウェブブラウザの設定操作の実行
        /// </summary>
        /// <param name="cmdId">コマンドID</param>
        /// <param name="cmdExecOpt">コマンド実行オプション</param>
        /// <param name="param">設定パラメータ</param>
        protected void ExecWB(OLECMDID cmdId, OLECMDEXECOPT cmdExecOpt, object param)
        {
            object pvaIn = param;
            object pvaOut = Type.Missing;
            webBrowser.ExecWB(cmdId, cmdExecOpt, ref pvaIn, ref pvaOut);
        }

        /// <summary>
        /// COMオブジェクトを解放する
        /// </summary>
        /// <param name="obj">COMオブジェクト</param>
        protected void ReleaseComObject(object obj)
        {
            // COMオブジェクト以外は処理しない
            if (obj == null)
            {
                return;
            }
            if (!Marshal.IsComObject(obj))
            {
                return;
            }

            // COMオブジェクトの解放
            Marshal.FinalReleaseComObject(obj);
        }

        #region IDisposable members

        /// <summary>
        /// リソース解放
        /// </summary>
        public void Dispose()
        {
            // リソースを解放する
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
                if (disposed)
                {
                    //既に呼びだしずみであるならばなんもしない
                    return;
                }

                disposed = true;

                if (disposing)
                {
                    // マネージリソースの解放
                }

                // ウィンドウハンドルが登録されている場合
                if (handle != IntPtr.Zero)
                {
                    // 文字サイズとズームを復元する
                    SetCharSize(charSizeOriginal);
                    SetZoom(zoomOriginal);
                }

                // アンマネージリソースの解放
                ReleaseComObject(body);
                ReleaseComObject(window);
                ReleaseComObject(document2);
                ReleaseComObject(document3);
                ReleaseComObject(document6);
                ReleaseComObject(webBrowser);
            }
        }

        #endregion
    }
}
