using System;
using System.Drawing;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using WinCap.Interop.Win32;

namespace WinCap.Models
{
    /// <summary>
    /// Bitmapの拡張機能を提供します。
    /// </summary>
    public static class BitmapExtensions
    {
        /// <summary>
        /// BitmapをBitmapSourceに変換します。
        /// </summary>
        /// <param name="source">変換するBitmap</param>
        /// <returns>変換後のBitmapSource</returns>
        public static BitmapSource ToBitmapSource(this Bitmap source)
        {
            IntPtr ptr = source.GetHbitmap();
            using (Disposable.Create(() => Gdi32.DeleteObject(ptr)))
            {
                return Imaging.CreateBitmapSourceFromHBitmap(
                        ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
        }
    }
}
