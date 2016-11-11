using System;
using System.Diagnostics;
using System.Drawing;

namespace WinCap.Models
{
    /// <summary>
    /// コントロールの情報を提供します。
    /// </summary>
    [DebuggerDisplay("Handle={Handle}, ClassName={ClassName}, Bounds={Bounds}")]
    public class ControlInfo
    {
        /// <summary>
        /// 空のコントロール情報
        /// </summary>
        public static readonly ControlInfo Empty = new ControlInfo();

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
