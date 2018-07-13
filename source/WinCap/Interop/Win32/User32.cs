using System;
using System.Runtime.InteropServices;
using System.Text;

namespace WinCap.Interop.Win32
{
    public static class User32
    {
        /// <summary>
        /// 指定ウィンドウと指定関係（またはオーナー）にあるウィンドウのハンドルを返します。
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="uCmd">関係</param>
        /// <returns>ウィンドウハンドル</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, GW uCmd);

        /// <summary>
        /// 指定ウィンドウの矩形情報を返す。
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="lpRect">矩形情報の参照</param>
        /// <returns>成功ならtrue、失敗ならfalseを返します。</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        /// <summary>
        /// 指定されたウィンドウに関する情報を取得します。
        /// </summary>
        /// <param name="hWnd">ウィンドウのハンドル</param>
        /// <param name="nIndex">取得する値のオフセット</param>
        /// <returns>要求したデータ</returns>
        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, GWL nIndex);

        /// <summary>
        /// フォアグラウンドウィンドウ（現在ユーザーが作業しているウィンドウ）のハンドルを返す。
        /// </summary>
        /// <returns>ウィンドウハンドル</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// 指定されたウィンドウの表示状態を調べます。
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <returns>表示状態</returns>
        [DllImport("user32.dll")]
        public static extern int IsWindowVisible(IntPtr hWnd);

        /// <summary>
        /// ウィンドウクラス名の取得。
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="lpClassName">クラス名の格納先</param>
        /// <param name="nMaxCount">クラス名バッファのサイズ</param>
        /// <returns>成功するとtrue、失敗するとfalseを返します。</returns>
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        /// <summary>
        /// 指定されたウィンドウのクライアント領域または画面全体を表すディスプレイデバイスコンテキストのハンドルを取得します。
        /// </summary>
        /// <param name="hWnd">ウィンドウのハンドル</param>
        /// <returns>デバイスコンテキスト</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        /// <summary>
        /// デバイスコンテキストを解放し、他のアプリケーションからつかえるようにします。
        /// </summary>
        /// <param name="hWnd">ウィンドウのハンドル</param>
        /// <param name="hDC">デバイスコンテキストのハンドル</param>
        /// <returns>成功するとtrue、失敗するとfalseを返します。</returns>
        [DllImport("user32.dll")]
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        /// <summary>
        /// システム全体で一意であることが保証される、1 つの新しいウィンドウメッセージを定義します。
        /// </summary>
        /// <param name="lpString">メッセージ文字列</param>
        /// <returns>成功した場合、0xC000~0xFFFのメッセージ識別子</returns>
        [DllImport("user32.dll")]
        public static extern uint RegisterWindowMessage(string lpString);

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
        public static extern bool SendMessageTimeout(IntPtr hWnd, uint Msg, Int32 wParam, Int32 lParam, uint fuFlags, uint uTimeout, ref UIntPtr lpdwResult);

        /// <summary>
        /// 仮想キーコードをスキャンコード、または文字の値（ASCII 値）へ変換します。
        /// </summary>
        /// <param name="uCode">キーの仮想キーコード</param>
        /// <param name="uMapType">実行したい変換の種類</param>
        /// <returns>スキャンコード、仮想キーコード、ASCII 値のいずれかが返ります。変換されないときは、0 が返ります。</returns>
        [DllImport("user32.dll")]
        public static extern int MapVirtualKey(uint uCode, uint uMapType);

        /// <summary>
        /// キーの名前を表す文字列を取得します。
        /// </summary>
        /// <param name="lParam">キーボードメッセージの第 2 パラメータ</param>
        /// <param name="str">キー名を保持するバッファへのポインタ</param>
        /// <param name="size">キー名を表す文字列の最大サイズ</param>
        /// <returns>成功ならコピーした文字列の長さ、失敗なら0が返ります。</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetKeyNameText(int lParam, [MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder str, int size);

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
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        /// <summary>
        /// 指定された点を含むディスプレイモニタのハンドルを返します。
        /// </summary>
        /// <param name="pt">目的の点</param>
        /// <param name="dwFlags">指定した点を含むモニタがなかった場合のフラグ</param>
        /// <returns>
        /// 指定した点を含むディスプレイモニタが見つかった場合は、そのディスプレイモニタの HMONITOR ハンドルが返ります。
        /// 指定した点を含むディスプレイモニタが見つからなかった場合は、dwFlags パラメータにより示される内容が返ります。
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr MonitorFromPoint(POINT pt, MonitorDefaultTo dwFlags);

        /// <summary>
        /// 指定した長方形領域との交差部分が最も広いディスプレイモニタのハンドルを返します。
        /// </summary>
        /// <param name="lpRect">RECT 構造体へのポインタ</param>
        /// <param name="dwFlags">指定した長方形領域と重なるモニタがなかった場合のフラグ</param>
        /// <returns>
        /// 指定した長方形領域と重なるディスプレイモニタが１台以上見つかった場合は、長方形領域との交差部分が最も広いディスプレイモニタの HMONITOR ハンドルが返ります。
        /// 指定した長方形領域と重なるディスプレイモニタが見つからなかった場合は、dwFlags パラメータにより示される内容が返ります。
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr MonitorFromRect(ref RECT lpRect, MonitorDefaultTo dwFlags);

        /// <summary>
        /// 指定したウィンドウの境界線により決まる長方形領域との交差部分が最も広いディスプレイモニタのハンドルを返します。
        /// </summary>
        /// <param name="hwnd">目的のウィンドウのハンドルを指定します。</param>
        /// <param name="dwFlags">指定したウィンドウと重なるディスプレイモニタが存在しなかった場合は、関数の戻り値の内容を示す値が格納されます。
        /// このパラメータには、次のいずれかの値が格納されます。</param>
        /// <returns>
        /// 指定したウィンドウと重なるディスプレイモニタが１台以上見つかった場合は、ウィンドウとの交差部分が最も広いディスプレイモニタの HMONITOR ハンドルが返ります。
        /// 指定したウィンドウと重なるディスプレイモニタが見つからなかった場合は、dwFlags パラメータにより示される内容が返ります。
        /// </returns>
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, MonitorDefaultTo dwFlags);

        /// <summary>
        /// システムワイド（システム全体に適用される）のホットキーを定義します。
        /// </summary>
        /// <param name="hWnd">ウィンドウのハンドル</param>
        /// <param name="id">ホットキーの識別子（0x0000～0xBFFF）</param>
        /// <param name="modKey">キー修飾子フラグ</param>
        /// <param name="key">仮想キーコード</param>
        /// <returns>成功ならtrue、失敗ならfalseを返します。</returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int modKey, int key);

        /// <summary>
        /// 呼び出し側スレッドが既に定義したホットキーを破棄します。
        /// </summary>
        /// <param name="hWnd">ウィンドウのハンドル</param>
        /// <param name="id">ホットキーの識別子（0x0000～0xBFFF）</param>
        /// <returns>成功ならtrue、失敗ならfalseを返します。</returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}
