using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinCap.Models
{
    /// <summary>
    /// 画面の補助機能を提供します。
    /// </summary>
    internal static class ScreenHelper
    {
        /// <summary>
        /// 画面全体の範囲を返却します。
        /// </summary>
        /// <returns>範囲</returns>
        public static Rectangle GetFullScreenBounds()
        {
            Rectangle bounds = new Rectangle();
            foreach (Screen screen in Screen.AllScreens)
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

        /// <summary>
        /// 画面範囲内に指定座標を含む画面を返却します。
        /// </summary>
        /// <param name="point">指定座標</param>
        /// <returns>画面</returns>
        public static Screen GetCurrentScreen(Point point)
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.Bounds.Contains(point))
                {
                    return screen;
                }
            }
            return null;
        }
    }
}
