using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinCap.Utility.Drawing
{
    /// <summary>
    /// スクリーンの拡張
    /// </summary>
    public static class ScreenExtensions
    {
        /// <summary>
        /// <see cref="screens"/>オブジェクトの範囲を返却する。
        /// </summary>
        /// <returns>範囲</returns>
        public static Rectangle GetBounds(this Screen[] screens)
        {
            Rectangle bounds = new Rectangle();
            foreach (Screen screen in screens)
            {
                bounds.X = Math.Min(bounds.X, screen.Bounds.Left);
                bounds.Y = Math.Min(bounds.Y, screen.Bounds.Top);
                bounds.Width = Math.Max(bounds.Width, screen.Bounds.Right);
                bounds.Height = Math.Max(bounds.Height, screen.Bounds.Bottom);
            }

            // オフセット分のサイズ補正
            bounds.Width -= bounds.X;
            bounds.Height -= bounds.Y;

            return bounds;
        }
    }
}
