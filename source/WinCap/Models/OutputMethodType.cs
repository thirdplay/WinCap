using System.Runtime.Serialization;

namespace WinCap.Models
{
    /// <summary>
    /// 出力方法の種類を表します。
    /// </summary>
    public enum OutputMethodType : int
    {
        /// <summary>
        /// クリップボード
        /// </summary>
        Clipboard,

        /// <summary>
        /// 画像ファイル
        /// </summary>
        ImageFile,
    }
}
