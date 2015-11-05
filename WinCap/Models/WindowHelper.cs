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
        /// ウィンドウ情報リストを表示順にソートして取得する
        /// </summary>
        /// <returns>ウィンドウ情報リスト</returns>
        public List<WindowInfo> GetWindowList()
        {
            List<WindowInfo> list = new List<WindowInfo>();

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
                if (NativeMethods.IsWindowVisible(hWnd) != 0)
                {
                    // 子ウィンドウ判定
                    IntPtr hWndChild = NativeMethods.GetWindow(hWnd, GW.CHILD);
                    if (hWndChild != IntPtr.Zero)
                    {
                        getWindowList(hWndChild, ref list);
                    }
                    list.Add(new WindowInfo(hWnd));
                }
            } while ((hWnd = NativeMethods.GetWindow(hWnd, GW.HWNDNEXT)) != IntPtr.Zero);

            return list;
        }

        /// <summary>
        /// 指定ウィンドウとそれに紐付く子ウィンドウを全て取得する
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="list">ウィンドウハンドルの格納先</param>
        protected void getWindowList(IntPtr hWnd, ref List<WindowInfo> list)
        {
            if (NativeMethods.IsWindowVisible(hWnd) != 0)
            {
                IntPtr hWndChild = NativeMethods.GetWindow(hWnd, GW.CHILD);
                if (hWndChild != IntPtr.Zero)
                {
                    getWindowList(hWndChild, ref list);
                }
                list.Add(new WindowInfo(hWnd));
            }

            if ((hWnd = NativeMethods.GetWindow(hWnd, GW.HWNDNEXT)) != IntPtr.Zero)
            {
                getWindowList(hWnd, ref list);
            }
        }
    }
}
