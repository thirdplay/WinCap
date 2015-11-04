using System;
using System.Collections.Generic;
using System.Drawing;
using WinCap.Win32;

namespace WinCap.Models
{
    /// <summary>
    /// ウィンドウヘルパー
    /// </summary>
    public class WindowHelper
    {
        /// <summary>
        /// 表示中のウィンドウ一覧をソートして取得する
        /// </summary>
        /// <returns>ウィンドウハンドル一覧</returns>
        public List<IntPtr> GetWindowHandles()
        {
            List<IntPtr> list = new List<IntPtr>();

            // フォアグラウンドウィンドウの取得
            IntPtr hWnd = NativeMethods.GetForegroundWindow();

            // 一番手前のウィンドウを取得する
            IntPtr hWndPrev;
            while ((hWndPrev = NativeMethods.GetWindow(hWnd, GW.HWNDPREV)) != IntPtr.Zero)
            {
                hWnd = hWndPrev;
            }

            do
            {
                // 対象ウィンドウか判定する
                if (isValidWindow(hWnd))
                {
                    // 子ウィンドウ判定
                    IntPtr hWndChild = NativeMethods.GetWindow(hWnd, GW.CHILD);
                    if (hWndChild != IntPtr.Zero)
                    {
                        getWindowHandles(hWndChild, list);
                    }
                    list.Add(hWnd);
                }
            } while ((hWnd = NativeMethods.GetWindow(hWnd, GW.HWNDNEXT)) != IntPtr.Zero);

            return list;
        }

        /// <summary>
        /// 指定ウィンドウに紐付く子ウィンドウを全て取得する
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="list">ウィンドウハンドルの格納先</param>
        protected void getWindowHandles(IntPtr hWnd, List<IntPtr> list)
        {
            // 対象ウィンドウか判定する
            if (isValidWindow(hWnd))
            {
                // 子判定
                IntPtr hWndChild = NativeMethods.GetWindow(hWnd, GW.CHILD);
                if (hWndChild != IntPtr.Zero)
                {
                    getWindowHandles(hWndChild, list);
                }
                list.Add(hWnd);
            }

            // 次ノード判定
            if ((hWnd = NativeMethods.GetWindow(hWnd, GW.HWNDNEXT)) != IntPtr.Zero)
            {
                getWindowHandles(hWnd, list);
            }
        }

        /// <summary>
        /// 指定ウィンドウをキャプチャ対象ウィンドウとするか返す
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <returns>対象ならtrue、それ以外はfalse</returns>
        protected bool isValidWindow(IntPtr hWnd)
        {
            // ウィンドウの表示状態を取得する
            if (NativeMethods.IsWindowVisible(hWnd) != 0)
            {
                // 矩形情報を取得する
                Rectangle rect = NativeMethods.GetWindowRect(hWnd);
                if (rect.Width != 0 && rect.Height != 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
