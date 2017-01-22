﻿using System;
using System.Drawing;
using WinCap.Interop;
using WinCap.Interop.Win32;
using WinCap.Models;

namespace WinCap.Capturers
{
    /// <summary>
    /// 画面をキャプチャする機能を提供します。
    /// </summary>
    public class ScreenCapturer
    {
        /// <summary>
        /// 画面全体をキャプチャする。
        /// </summary>
        /// <returns>ビットマップ</returns>
        public Bitmap CaptureFullScreen()
        {
            return CaptureBounds(ScreenHelper.GetFullScreenBounds());
        }

        /// <summary>
        /// 指定範囲の画面をキャプチャする。
        /// </summary>
        /// <param name="bounds">範囲</param>
        /// <returns>ビットマップ</returns>
        public Bitmap CaptureBounds(Rectangle bounds)
        {
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            IntPtr screenDC = User32.GetDC(IntPtr.Zero);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                IntPtr hDC = g.GetHdc();
                Gdi32.BitBlt(hDC, 0, 0, bitmap.Width, bitmap.Height,
                    screenDC, bounds.X, bounds.Y, TernaryRasterOperations.SRCCOPY);
                g.ReleaseHdc(hDC);
            }
            User32.ReleaseDC(IntPtr.Zero, screenDC);

            return bitmap;
        }
    }
}
