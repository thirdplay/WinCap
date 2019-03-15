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
        /// <param name="self">変換するクライアント座標</param>
        /// <returns>変換後のスクリーン座標</returns>
        public static System.Windows.Point ToPoint(this System.Drawing.Point self)
        {
            return new System.Windows.Point(self.X, self.Y);
        }
    }
}
