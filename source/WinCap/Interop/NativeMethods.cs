﻿using System;
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
        /// <returns>成功なら0以外</returns>
        [DllImport("user32.dll")]
        public static extern int SendMessageTimeout(IntPtr hWnd, uint Msg, Int32 wParam, Int32 lParam, uint fuFlags, uint uTimeout, ref UIntPtr lpdwResult);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="lResult"></param>
        /// <param name="riid">GUID</param>
        /// <param name="wParam">メッセージ特有の追加情報</param>
        /// <param name="ppvObject"></param>
        /// <returns></returns>
        [DllImport("oleacc.dll")]
        public static extern int ObjectFromLresult(UIntPtr lResult, ref Guid riid, Int32 wParam, ref MSHTML.IHTMLDocument2 ppvObject);

        /// <summary>
        /// 指定ウィンドウハンドルの範囲を取得します。
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        /// <returns>ウィンドウの範囲</returns>
        public static Rectangle GetWindowBounds(IntPtr handle)
        {
            // Vista以降の場合、DWMでウィンドウサイズを取得
            RECT rect = new RECT();
            if (Environment.OSVersion.Version.Major >= 6)
            {
                if (NativeMethods.DwmGetWindowAttribute(handle, (int)DWMWA.EXTENDED_FRAME_BOUNDS, ref rect, Marshal.SizeOf(typeof(RECT))) == 0)
                {
                    Rectangle rectangle = new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
                    if (rectangle.Width > 0 && rectangle.Height > 0)
                    {
                        return rectangle;
                    }
                }
            }

            // ウィンドウサイズの取得
            if (NativeMethods.GetWindowRect(handle, ref rect) != 0)
            {
                return new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
            }
            return Rectangle.Empty;
        }
    }
}