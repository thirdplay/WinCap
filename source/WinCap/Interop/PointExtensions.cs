using System.Drawing;
using WinCap.Capturers;

namespace WinCap.Interop
{
    /// <summary>
    /// <see cref="Point"/>構造体の拡張機能を提供します。
    /// </summary>
    public static class PointExtensions
    {
        /// <summary>
        /// クライアント座標をスクリーン座標に変換します。
        /// </summary>
        /// <param name="self">変換するクライアント座標</param>
        /// <returns>変換後のスクリーン座標</returns>
        public static Point PointToScreen(this Point self)
        {
            var point = ScreenHelper.GetScreenOrigin();
            return new Point(self.X - point.X, self.Y - point.Y);
        }
    }
}
