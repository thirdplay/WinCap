using System.Runtime.Serialization;

namespace WinCap.Models
{
    /// <summary>
    /// 出力形式の種類を表します。
    /// </summary>
    public enum OutputFormatType : int
    {
        /// <summary>
        /// BMP
        /// </summary>
        Bmp,

        /// <summary>
        /// JPG
        /// </summary>
        Jpg,

        /// <summary>
        /// PNG
        /// </summary>
        Png,

        /// <summary>
        /// GIF
        /// </summary>
        Gif,
    }
}
