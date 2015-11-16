using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using WinCap.Win32;

namespace WinCap.Utilities.Drawing
{
    /// <summary>
    /// Bitmap拡張
    /// </summary>
    public static class BitmapExtensions
    {
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
                NativeMethods.DeleteObject(ptr);
            }
            return bs;
        }
    }
}
