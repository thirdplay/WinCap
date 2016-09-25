using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace WinCap.Utility.Drawing
{
    /// <summary>
    /// Bitmap拡張
    /// </summary>
    public static class BitmapExtensions
    {
        /// <summary>
        /// Objectを削除する。
        /// </summary>
        /// <param name="hObject">オブジェクトハンドル</param>
        /// <returns></returns>
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// <see cref="Bitmap"/>オブジェクトを<see cref="BitmapSource"/>オブジェクトに変換する。
        /// </summary>
        /// <param name="source">Bitmapオブジェクト</param>
        /// <returns>BitmapSourceオブジェクト</returns>
        public static BitmapSource ToBitmapSource(this Bitmap source)
        {
            IntPtr ptr = source.GetHbitmap();
            BitmapSource bs;
            try
            {
                bs = Imaging.CreateBitmapSourceFromHBitmap(ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(ptr);
            }
            return bs;
        }
    }
}
