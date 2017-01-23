using System;
using System.Runtime.InteropServices;

namespace WinCap.Interop.Win32
{
    public static class Gdi32
    {
        /// <summary>
        /// Objectを削除する。
        /// </summary>
        /// <param name="hObject">オブジェクトハンドル</param>
        /// <returns>成功なら0、失敗なら0以外を返します。</returns>
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(IntPtr hObject);

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
        public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);
    }
}
