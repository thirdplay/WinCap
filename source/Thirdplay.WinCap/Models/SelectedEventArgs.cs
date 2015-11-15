using System;

namespace Thirdplay.WinCap.Models
{
    /// <summary>
    /// 選択イベント引数
    /// </summary>
    public class SelectedEventArgs
    {
        /// <summary>
        /// ウィンドウハンドル
        /// </summary>
        public IntPtr Handle { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        public SelectedEventArgs(IntPtr handle)
        {
            this.Handle = handle;
        }
    }
}
