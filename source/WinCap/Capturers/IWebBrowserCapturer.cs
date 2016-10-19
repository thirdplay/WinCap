using System;
using System.Drawing;

namespace WinCap.Capturers
{
    /// <summary>
    /// ウェブブラウザのキャプチャ機能を提供するインターフェイス。
    /// </summary>
    public interface IWebBrowserCapturer
    {
        /// <summary>
        /// スクロールの遅延時間
        /// </summary>
        int ScrollDelayTime { get; set; }

        /// <summary>
        /// スクロールウィンドウ開始時のページ先頭移動フラグ
        /// </summary>
        bool IsScrollWindowPageTop { get; set; }

        /// <summary>
        /// キャプチャ可能かどうか判定します。
        /// </summary>
        /// <param name="className">ウィンドウクラス名</param>
        /// <returns>判定結果</returns>
        bool CanCapture(string className);

        /// <summary>
        /// ウェブブラウザをキャプチャします。
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <returns>ビットマップ</returns>
        Bitmap Capture(IntPtr hWnd);
    }
}
