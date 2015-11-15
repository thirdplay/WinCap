using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using WinCap.Win32;

namespace WinCap.Models
{
    /// <summary>
    /// ウィンドウヘルパー
    /// </summary>
    internal static class WindowHelper
    {
        /// <summary>
        /// ウィンドウハンドルリストを表示順にソートして取得する
        /// </summary>
        /// <returns>ウィンドウハンドルリスト</returns>
        public static List<IntPtr> GetHandleList()
        {
            List<IntPtr> list = new List<IntPtr>();

            // 一番手前のウィンドウを取得する
            IntPtr handle = NativeMethods.GetForegroundWindow();
            IntPtr hWndPrev;
            while ((hWndPrev = NativeMethods.GetWindow(handle, GW.HWNDPREV)) != IntPtr.Zero)
            {
                handle = hWndPrev;
            }

            do
            {
                // 対象ウィンドウか判定する
                if (isValidWindow(handle))
                {
                    // 子ウィンドウ判定
                    IntPtr hWndChild = NativeMethods.GetWindow(handle, GW.CHILD);
                    if (hWndChild != IntPtr.Zero)
                    {
                        getWindowList(hWndChild, ref list);
                    }
                    list.Add(handle);
                }
            } while ((handle = NativeMethods.GetWindow(handle, GW.HWNDNEXT)) != IntPtr.Zero);

            // 先頭の要素（HwndWrapper）は除外する
            list.RemoveAt(0);
            return list;
        }

        /// <summary>
        /// 指定ウィンドウとそれに紐付く子ウィンドウを全て取得する
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        /// <param name="list">ウィンドウハンドルの格納先</param>
        private static void getWindowList(IntPtr handle, ref List<IntPtr> list)
        {
            if (isValidWindow(handle))
            {
                IntPtr hWndChild = NativeMethods.GetWindow(handle, GW.CHILD);
                if (hWndChild != IntPtr.Zero)
                {
                    getWindowList(hWndChild, ref list);
                }
                list.Add(handle);
            }

            if ((handle = NativeMethods.GetWindow(handle, GW.HWNDNEXT)) != IntPtr.Zero)
            {
                getWindowList(handle, ref list);
            }
        }

        /// <summary>
        /// 指定ウィンドウをキャプチャ対象ウィンドウとするか返す。
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        /// <returns>対象ならtrue、それ以外はfalse</returns>
        private static bool isValidWindow(IntPtr handle)
        {
            // ウィンドウの表示状態を取得する
            int visible = NativeMethods.IsWindowVisible(handle);
            if (visible != 0)
            {
                // 矩形情報を取得する
                Rectangle rect = GetWindowBounds(handle);
                if (rect.Width > 0 && rect.Height > 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 指定ウィンドウハンドルの範囲を取得する。
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        /// <returns>ウィンドウの範囲</returns>
        public static Rectangle GetWindowBounds(IntPtr handle)
        {
            // Vista以降の場合、DWMでウィンドウサイズを取得する
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

        /// <summary>
        /// 指定ウィンドウハンドルのクラス名を取得する。
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        /// <returns>クラス名</returns>
        public static string GetClassName(IntPtr handle)
        {
            StringBuilder builder = new StringBuilder(256);
            if (NativeMethods.GetClassName(handle, builder, builder.Capacity) != 0)
            {
                return builder.ToString();
            }
            return "";
        }
    }
}
