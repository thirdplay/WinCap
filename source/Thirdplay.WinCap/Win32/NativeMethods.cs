using System;
using System.Runtime.InteropServices;
using System.Text;

namespace WinCap.Win32
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
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// ウィンドウの属性情報を返す。
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="dwAttribute">属性の識別子</param>
        /// <param name="pvAttribute">属性情報の参照</param>
        /// <param name="cbAttribute">属性情報のサイズ</param>
        /// <returns>成功なら0、失敗なら0以外を返す</returns>
        [DllImport("Dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(IntPtr hWnd, int dwAttribute, ref RECT pvAttribute, int cbAttribute);

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
        /// <returns>成功なら0以外、失敗なら0を返す</returns>
        [DllImport("user32.dll")]
        public static extern int GetWindowRect(IntPtr hWnd, ref RECT lpRect);

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
        /// <returns>成功すると0以外、失敗すると0</returns>
        [DllImport("user32.dll", CharSet=CharSet.Unicode)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        public static string GetClassName(IntPtr handle)
        {
            StringBuilder builder = new StringBuilder(256);
            if (GetClassName(handle, builder, builder.Capacity) != 0)
            {
                return builder.ToString();
            }
            return "";
        }

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
        /// <returns>成功すると0以外、失敗すると0が返ります。</returns>
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

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
        /// <returns>成功すると1、失敗すると0が返ります。</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
        
        /// <summary>
        /// ホットキーを登録する。
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="id">ホットキーID</param>
        /// <param name="fsModifiers">装飾キー</param>
        /// <param name="vk">仮想キー</param>
        /// <returns>成功なら0以外、失敗なら0</returns>
        [DllImport("user32.dll")]
        public static extern int RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        /// <summary>
        /// ホットキーを登録解除する。
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="id">ホットキーID</param>
        /// <returns>成功なら0以外、失敗なら0</returns>
        [DllImport("user32.dll")]
        public static extern int UnregisterHotKey(IntPtr hWnd, int id);
    }
}
