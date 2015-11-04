using System;
using System.Drawing;
using System.Runtime.InteropServices;

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
        public static Rectangle GetWindowRect(IntPtr hWnd)
        {
            RECT rect = new RECT();

            // Vista以降の場合、DWMでウィンドウサイズを取得する
            if (Environment.OSVersion.Version.Major >= 6)
            {
                if (DwmGetWindowAttribute(hWnd, (int)DWMWA.EXTENDED_FRAME_BOUNDS, ref rect, Marshal.SizeOf(typeof(Win32.RECT))) == 0)
                {
                    return new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
                    //Rectangle rectangle = new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
                    //if (rectangle.Width != 0 && rectangle.Height != 0)
                    //{
                    //    return rectangle;
                    //}
                }
            }

            // ウィンドウサイズの取得
            if (GetWindowRect(hWnd, ref rect) != 0)
            {
                return new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
            }
            return Rectangle.Empty;
        }

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
