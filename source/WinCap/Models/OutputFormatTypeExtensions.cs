using System;
using System.Drawing.Imaging;

namespace WinCap.Models
{
    /// <summary>
    /// 出力形式の拡張機能を提供します。
    /// </summary>
    public static class OutputFormatTypeExtensions
    {
        /// <summary>
        /// 出力形式をイメージファイルの形式ーに変換します。
        /// </summary>
        /// <param name="type">出力形式</param>
        /// <returns>イメージファイルの形式</returns>
        public static ImageFormat ToImageFormat(this OutputFormatType type)
        {
            switch (type)
            {
                case OutputFormatType.Bmp: return ImageFormat.Bmp;
                case OutputFormatType.Jpg: return ImageFormat.Jpeg;
                case OutputFormatType.Png: return ImageFormat.Png;
                case OutputFormatType.Gif: return ImageFormat.Gif;
            }
            throw new ArgumentException(nameof(type));
        }
    }
}
