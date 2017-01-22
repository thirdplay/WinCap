using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace WinCap.Models
{
    /// <summary>
    /// Bitmapの拡張機能を提供します。
    /// </summary>
    public static class BitmapExtensions
    {
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// BitmapをBitmapSourceに変換します。
        /// </summary>
        /// <param name="source">変換するBitmap</param>
        /// <returns>返還後のBitmapSource</returns>
        public static BitmapSource ToBitmapSource(this Bitmap source)
        {
            IntPtr ptr = source.GetHbitmap();
            BitmapSource bs;
            try
            {
                bs = Imaging.CreateBitmapSourceFromHBitmap(
                        ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(ptr);
            }
            return bs;
        }
    }
}
