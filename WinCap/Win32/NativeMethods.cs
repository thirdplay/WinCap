using System;
using System.Runtime.InteropServices;

namespace WinCap.Win32
{
    /// <summary>
    /// ネイティブメソッド
    /// </summary>
    internal static class NativeMethods
    {
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
