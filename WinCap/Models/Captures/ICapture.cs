using System.Drawing;

namespace WinCap.Models.Captures
{
    /// <summary>
    /// キャプチャインターフェイス
    /// </summary>
    public interface ICapture
    {
        /// <summary>
        /// キャプチャした結果をビットマップで返却する。
        /// </summary>
        /// <returns>ビットマップ</returns>
        Bitmap Capture();
    }
}
