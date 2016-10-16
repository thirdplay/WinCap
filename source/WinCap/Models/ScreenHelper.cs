﻿using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinCap.Models
{
    /// <summary>
    /// スクリーンヘルパー
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
    }
}