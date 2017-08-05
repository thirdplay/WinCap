using System;
using System.Drawing;
using System.Media;
using System.Threading;
using System.Windows;
using WinCap.Models;
using WinCap.Serialization;
using ProductInfo = WinCap.Properties.ProductInfo;

namespace WinCap.Capturers
{
    /// <summary>
    /// キャプチャする機能を提供する基底クラス。
    /// </summary>
    /// <typeparam name="TTarget">キャプチャ対象の型</typeparam>
    public abstract class CapturerBase<TTarget>
    {
        /// <summary>
        /// 画面をキャプチャします。
        /// </summary>
        public void Capture()
        {
            try
            {
                // キャプチャ対象取得
                var target = GetCaptureTarget();
                if (target == null)
                {
                    return;
                }

                // キャプチャ遅延時間
                if (Settings.General.CaptureDelayTime > 0)
                {
                    Thread.Sleep(Settings.General.CaptureDelayTime);
                }

                // キャプチャ
                using (var bitmap = CaptureCore(target))
                {
                    var settings = Settings.Output;
                    if (settings.OutputMethodType == OutputMethodType.Clipboard)
                    {
                        Clipboard.SetImage(bitmap.ToBitmapSource());
                    }
                    else if (settings.OutputMethodType == OutputMethodType.ImageFile)
                    {
                        bitmap.SaveCaptureImage();
                    }
                }

                // キャプチャ音再生
                if (Settings.General.IsPlaySeWhenCapture)
                {
                    SystemSounds.Asterisk.Play();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ProductInfo.Title);
                Application.ReportException(this, ex, false);
            }
        }

        /// <summary>
        /// キャプチャ対象を取得します。
        /// </summary>
        /// <returns>キャプチャ対象</returns>
        protected abstract TTarget GetCaptureTarget();

        /// <summary>
        /// キャプチャのコア処理。
        /// </summary>
        /// <param name="target">キャプチャ対象</param>
        /// <returns>キャプチャ画像</returns>
        protected abstract Bitmap CaptureCore(TTarget target);
    }
}
