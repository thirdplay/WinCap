using System;
using System.Drawing;
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
        /// ウィンドウの属性情報を返す
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
        /// 指定ウィンドウの矩形情報を返す
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="lpRect">矩形情報の参照</param>
        /// <returns>成功なら0以外、失敗なら0を返す</returns>
        [DllImport("user32.dll")]
        public static extern int GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        /// <summary>
        /// フォアグラウンドウィンドウ（現在ユーザーが作業しているウィンドウ）のハンドルを返す
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
        /// ウィンドウクラス名の取得
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="lpClassName">クラス名の格納先</param>
        /// <param name="nMaxCount">クラス名バッファのサイズ</param>
        /// <returns>成功すると0以外、失敗すると0</returns>
        [DllImport("user32.dll", CharSet=CharSet.Unicode)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        /// <summary>
        /// ホットキーを登録する
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="ID">ホットキーID</param>
        /// <param name="MOD_KEY">装飾キー</param>
        /// <param name="KEY">キー</param>
        /// <returns>成功なら0以外、失敗なら0</returns>
        [DllImport("user32.dll")]
        public static extern int RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        /// <summary>
        /// ホットキーを登録解除する
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="ID">ホットキーID</param>
        /// <returns>成功なら0以外、失敗なら0</returns>
        [DllImport("user32.dll")]
        public static extern int UnregisterHotKey(IntPtr hWnd, int id);
    }
}
