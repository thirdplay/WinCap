using System;
using System.Drawing;

namespace WinCap.Models
{
    /// <summary>
    /// コントロール情報
    /// </summary>
    public class ControlInfo
    {
        #region プロパティ
        /// <summary>
        /// ウィンドウハンドル
        /// </summary>
        public IntPtr Handle { get; set; } = IntPtr.Zero;

        /// <summary>
        /// クラス名
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 位置とサイズ
        /// </summary>
        public Rectangle Bounds { get; set; } = Rectangle.Empty;

        /// <summary>
        /// 位置
        /// </summary>
        public Point Locale { get { return this.Bounds.Location; } }

        /// <summary>
        /// サイズ
        /// </summary>
        public Size Size { get { return this.Bounds.Size; } }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ControlInfo()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        /// <param name="className">クラス名</param>
        /// <param name="bounds">位置とサイズ</param>
        public ControlInfo(IntPtr handle, string className, Rectangle bounds)
        {
            this.Handle = handle;
            this.ClassName = className;
            this.Bounds = bounds;
        }
    }
}
