using System;
using System.Drawing;
using System.IO;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using WinCap.Interop.Win32;
using WinCap.Serialization;
using Resources = WinCap.Properties.Resources;

namespace WinCap.Models
{
    /// <summary>
    /// Bitmapの拡張機能を提供します。
    /// </summary>
    public static class BitmapExtensions
    {
        /// <summary>
        /// BitmapをBitmapSourceに変換します。
        /// </summary>
        /// <param name="source">変換するBitmap</param>
        /// <returns>変換後のBitmapSource</returns>
        public static BitmapSource ToBitmapSource(this Bitmap source)
        {
            IntPtr ptr = source.GetHbitmap();
            using (Disposable.Create(() => Gdi32.DeleteObject(ptr)))
            {
                return Imaging.CreateBitmapSourceFromHBitmap(
                        ptr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
        }

        /// <summary>
        /// キャプチャ画像を保存します。
        /// </summary>
        /// <param name="bitmap">画像</param>
        public static void SaveCaptureImage(this Bitmap bitmap)
        {
            // 出力形式とファイルパスの設定
            var settings = Settings.Output;
            OutputFormatType outputFormatType = settings.OutputFormatType;
            string fileExtension = outputFormatType.GetExtension();
            string fileName = FileHelper.CreateFileName(settings.OutputFolder, fileExtension, settings.OutputFileNamePattern);
            string filePath = fileName + fileExtension;
            if (!string.IsNullOrEmpty(settings.OutputFolder))
            {
                filePath = Path.Combine(settings.OutputFolder, filePath);
            }

            // 自動保存の場合
            if (settings.IsAutoSaveImage)
            {
                // 出力フォルダがない場合は作成する
                if (!Directory.Exists(settings.OutputFolder))
                {
                    Directory.CreateDirectory(settings.OutputFolder);
                }
            }
            else
            {
                // ファイルの保存場所を選択する
                using (var dialog = new System.Windows.Forms.SaveFileDialog()
                {
                    Filter = Resources.Services_SaveImageFileDialog_Filter,
                    InitialDirectory = settings.OutputFolder,
                    FilterIndex = (int)outputFormatType + 1,
                    Title = Resources.Services_SaveImageFileDialog_Title,
                    FileName = Path.GetFileName(fileName)
                })
                {
                    if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        return;
                    }
                    filePath = dialog.FileName;
                    outputFormatType = (OutputFormatType)dialog.FilterIndex;
                }
            }

            // 画像ファイルに保存
            bitmap.Save(filePath, outputFormatType.ToImageFormat());
        }
    }
}
