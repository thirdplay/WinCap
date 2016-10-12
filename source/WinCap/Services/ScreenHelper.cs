﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinCap.Services
{
    /// <summary>
    /// スクリーンヘルパー
    /// </summary>
    internal static class ScreenHelper
    {
        /// <summary>
        /// デスクトップ全体の範囲を返却します。
        /// </summary>
        /// <returns>範囲</returns>
        public static Rectangle GetDesktopBounds()
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
    }
}