using System.Diagnostics;
using WinCap.Capturers;

namespace WinCap.Models
{
    /// <summary>
    /// <see cref="Point"/>構造体の拡張機能を提供します。
    /// </summary>
    public static class PointExtensions
    {
        /// <summary>
        /// <see cref="System.Drawing.Point"/>構造体を<see cref="System.Windows.Point"/>構造体に変換します。
        /// </summary>
        /// <param name="self">System.Drawing.Point構造体</param>
        /// <returns>System.Windows.Point構造体</returns>
        public static System.Windows.Point ToPoint(this System.Drawing.Point self)
        {
            return new System.Windows.Point(self.X, self.Y);
        }

        /// <summary>
        /// <see cref="System.Windows.Point"/>構造体を<see cref="System.Drawing.Point"/>構造体に変換します。
        /// </summary>
        /// <param name="self">System.Windows.Point構造体</param>
        /// <returns>System.Drawing.Point構造体</returns>
        public static System.Drawing.Point ToPoint(this System.Windows.Point self)
        {
            return new System.Drawing.Point((int)self.X, (int)self.Y);
        }

        /// <summary>
        /// コントロール座標をスクリーン座標に変換します。
        /// </summary>
        /// <param name="self">System.Windows.Point構造体</param>
        /// <returns>変換後の座標</returns>
        public static System.Drawing.Point PointToScreen(this System.Drawing.Point self)
        {
            var origin = ScreenHelper.GetScreenOrigin();
            return new System.Drawing.Point(self.X + origin.X, self.Y + origin.Y);
        }

        /// <summary>
        /// スクリーン座標をコントロール座標に変換します。
        /// </summary>
        /// <param name="self">System.Windows.Point構造体</param>
        /// <returns>変換後の座標</returns>
        public static System.Drawing.Point PointToControl(this System.Drawing.Point self)
        {
            var origin = ScreenHelper.GetScreenOrigin();
            return new System.Drawing.Point(self.X - origin.X, self.Y - origin.Y);
        }
    }
}
