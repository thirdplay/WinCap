using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace WinCap.Interop
{
    /// <summary>
    /// ネイティブメソッド
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// Objectを削除する。
        /// </summary>
        /// <param name="hObject">オブジェクトハンドル</param>
        /// <returns>成功なら0、失敗なら0以外を返します。</returns>
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// ウィンドウの属性情報を返す。
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="dwAttribute">属性の識別子</param>
        /// <param name="pvAttribute">属性情報の参照</param>
        /// <param name="cbAttribute">属性情報のサイズ</param>
        /// <returns>成功なら0、失敗なら0以外を返します。</returns>
        [DllImport("Dwmapi.dll")]
        internal static extern int DwmGetWindowAttribute(IntPtr hWnd, int dwAttribute, ref RECT pvAttribute, int cbAttribute);

        /// <summary>
        /// 指定ウィンドウと指定関係（またはオーナー）にあるウィンドウのハンドルを返します。
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="uCmd">関係</param>
        /// <returns>ウィンドウハンドル</returns>
        [DllImport("user32.dll")]
        internal static extern IntPtr GetWindow(IntPtr hWnd, GW uCmd);

        /// <summary>
        /// 指定ウィンドウの矩形情報を返す。
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="lpRect">矩形情報の参照</param>
        /// <returns>成功ならtrue、失敗ならfalseを返します。</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        /// <summary>
        /// フォアグラウンドウィンドウ（現在ユーザーが作業しているウィンドウ）のハンドルを返す。
        /// </summary>
        /// <returns>ウィンドウハンドル</returns>
        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// 指定されたウィンドウの表示状態を調べます。
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <returns>表示状態</returns>
        [DllImport("user32.dll")]
        internal static extern int IsWindowVisible(IntPtr hWnd);

        /// <summary>
        /// ウィンドウクラス名の取得。
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="lpClassName">クラス名の格納先</param>
        /// <param name="nMaxCount">クラス名バッファのサイズ</param>
        /// <returns>成功するとtrue、失敗するとfalseを返します。</returns>
        [DllImport("user32.dll", CharSet=CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        /// <summary>
        /// ビットブロック転送を行います。
        /// </summary>
        /// <param name="hdc">コピー先デバイスコンテキストのハンドル</param>
        /// <param name="nXDest">コピー先長方形の左上隅の x 座標</param>
        /// <param name="nYDest">コピー先長方形の左上隅の y 座標</param>
        /// <param name="nWidth">コピー先長方形の幅</param>
        /// <param name="nHeight">コピー先長方形の高さ</param>
        /// <param name="hdcSrc">コピー元デバイスコンテキストのハンドル</param>
        /// <param name="nXSrc">コピー元長方形の左上隅の x 座標</param>
        /// <param name="nYSrc">コピー元長方形の左上隅の y 座標</param>
        /// <param name="dwRop">ラスタオペレーションコード</param>
        /// <returns>成功するとtrue、失敗するとfalseを返します。</returns>
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        /// <summary>
        /// 指定されたウィンドウのクライアント領域または画面全体を表すディスプレイデバイスコンテキストのハンドルを取得します。
        /// </summary>
        /// <param name="hWnd">ウィンドウのハンドル</param>
        /// <returns>デバイスコンテキスト</returns>
        [DllImport("user32.dll")]
        internal static extern IntPtr GetDC(IntPtr hWnd);

        /// <summary>
        /// デバイスコンテキストを解放し、他のアプリケーションからつかえるようにします。
        /// </summary>
        /// <param name="hWnd">ウィンドウのハンドル</param>
        /// <param name="hDC">デバイスコンテキストのハンドル</param>
        /// <returns>成功するとtrue、失敗するとfalseを返します。</returns>
        [DllImport("user32.dll")]
        internal static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        /// <summary>
        /// システム全体で一意であることが保証される、1 つの新しいウィンドウメッセージを定義します。
        /// </summary>
        /// <param name="lpString">メッセージ文字列</param>
        /// <returns>成功した場合、0xC000~0xFFFのメッセージ識別子</returns>
        [DllImport("user32.dll")]
        internal static extern uint RegisterWindowMessage(string lpString);

        /// <summary>
        /// 指定されたメッセージを、1 つまたは複数のウィンドウへ送信します。
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="Msg">メッセージ</param>
        /// <param name="wParam">メッセージ特有の追加情報</param>
        /// <param name="lParam">メッセージ特有の追加情報</param>
        /// <param name="fuFlags">メッセージ送信方法</param>
        /// <param name="uTimeout">タイムアウト期間（ミリ秒）</param>
        /// <param name="lpdwResult">メッセージ処理結果</param>
        /// <returns>成功するとtrue、失敗するとfalseを返します。</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SendMessageTimeout(IntPtr hWnd, uint Msg, Int32 wParam, Int32 lParam, uint fuFlags, uint uTimeout, ref UIntPtr lpdwResult);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="lResult"></param>
        /// <param name="riid">GUID</param>
        /// <param name="wParam">メッセージ特有の追加情報</param>
        /// <param name="ppvObject"></param>
        /// <returns></returns>
        [DllImport("oleacc.dll")]
        internal static extern int ObjectFromLresult(UIntPtr lResult, ref Guid riid, Int32 wParam, ref MSHTML.IHTMLDocument2 ppvObject);

        /// <summary>
        /// 仮想キーコードをスキャンコード、または文字の値（ASCII 値）へ変換します。
        /// </summary>
        /// <param name="uCode">キーの仮想キーコード</param>
        /// <param name="uMapType">実行したい変換の種類</param>
        /// <returns>スキャンコード、仮想キーコード、ASCII 値のいずれかが返ります。変換されないときは、0 が返ります。</returns>
        [DllImport("user32.dll")]
        internal static extern int MapVirtualKey(uint uCode, uint uMapType);

        /// <summary>
        /// キーの名前を表す文字列を取得します。
        /// </summary>
        /// <param name="lParam">キーボードメッセージの第 2 パラメータ</param>
        /// <param name="str">キー名を保持するバッファへのポインタ</param>
        /// <param name="size">キー名を表す文字列の最大サイズ</param>
        /// <returns>成功ならコピーした文字列の長さ、失敗なら0が返ります。</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetKeyNameText(int lParam, [MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder str, int size);

        /// <summary>
        /// 子ウィンドウ、ポップアップウィンドウ、またはトップレベルウィンドウのサイズ、位置、および Z オーダーを変更します。
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="hWndInsertAfter">Z オーダーを決めるためのウィンドウハンドル</param>
        /// <param name="X">ウィンドウの左上端の新しい x 座標（クライアント座標）</param>
        /// <param name="Y">ウィンドウの左上端の新しい y 座標（クライアント座標）</param>
        /// <param name="cx">ウィンドウの新しい幅（ピクセル単位）</param>
        /// <param name="cy">ウィンドウの新しい高さ（ピクセル単位）</param>
        /// <param name="uFlags">ウィンドウのサイズと位置の変更に関するフラグを</param>
        /// <returns>成功ならtrue、失敗ならfalseを返します。</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    }
}
