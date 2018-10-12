using System.Drawing;

namespace WinCap.Interop
{
    /// <summary>
    /// 四角形の拡張機能を提供します。
    /// </summary>
    public static class RectangleExtensions
    {
        /// <summary>
        /// 位置とサイズを物理ピクセル値に変換します。
        /// </summary>
        /// <param name="rect">変換前の位置とサイズ</param>
        /// <param name="dpi">DPI情報</param>
        /// <returns>変換後の位置とサイズ</returns>
        public static Rectangle ToPhysicalPixel(this Rectangle rect, Dpi dpi)
        {
            return new Rectangle(
                (int)(rect.X * dpi.ScaleX),
                (int)(rect.Y * dpi.ScaleY),
                (int)(rect.Width * dpi.ScaleX),
                (int)(rect.Height * dpi.ScaleY)
            );
        }

        /// <summary>
        /// 位置とサイズを論理ピクセル値に変換します。
        /// </summary>
        /// <param name="rect">変換前の位置とサイズ</param>
        /// <returns>変換後の位置とサイズ</returns>
        public static Rectangle ToLogicalPixel(this Rectangle rect)
        {
            return rect.ToLogicalPixel(PerMonitorDpi.GetDpi(rect));
        }

        /// <summary>
        /// 位置とサイズを論理ピクセル値に変換します。
        /// </summary>
        /// <param name="rect">変換前の位置とサイズ</param>
        /// <param name="dpi">DPI情報</param>
        /// <returns>変換後の位置とサイズ</returns>
        public static Rectangle ToLogicalPixel(this Rectangle rect, Dpi dpi)
        {
            return new Rectangle(
                (int)(rect.X * (1 / dpi.ScaleX)),
                (int)(rect.Y * (1 / dpi.ScaleY)),
                (int)(rect.Width * (1 / dpi.ScaleX)),
                (int)(rect.Height * (1 / dpi.ScaleY))
            );
        }
    }
}
