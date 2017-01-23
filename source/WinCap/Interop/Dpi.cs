using System.Diagnostics;

namespace WinCap.Interop
{
    /// <summary>
    /// モニターの DPI (dots per inch) を表します。
    /// </summary>
    [DebuggerDisplay("X = {X} ({ScaleX}), Y = {Y} ({ScaleY})")]
    public struct Dpi
    {
        /// <summary>
        /// デフォルトDPI。
        /// </summary>
        public static readonly Dpi Default = new Dpi(96, 96);

        /// <summary>
        /// X軸のDPI倍率。
        /// </summary>
        private double? scaleX;

        /// <summary>
        /// Y軸のDPI倍率。
        /// </summary>
        private double? scaleY;

        /// <summary>
        /// X軸のDPIを取得します。
        /// </summary>
        public uint X { get; }

        /// <summary>
        /// Y軸のDPIを取得します。
        /// </summary>
        public uint Y { get; }

        /// <summary>
        /// X軸のDPI倍率を取得します。
        /// </summary>
        public double ScaleX => this.scaleX ?? (this.scaleX = this.X / (double)Default.X).Value;

        /// <summary>
        /// Y軸のDPI倍率を取得します。
        /// </summary>
        public double ScaleY => this.scaleY ?? (this.scaleY = this.Y / (double)Default.Y).Value;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="x">X軸のDPI</param>
        /// <param name="y">Y軸のDPI</param>
        public Dpi(uint x, uint y)
            : this()
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// オブジェクトが等価かどうかを判断します。
        /// </summary>
        /// <param name="dpi1">比較対象1</param>
        /// <param name="dpi2">比較対象2</param>
        /// <returns>等価の場合はtrue。それ以外はfalse。</returns>
        public static bool operator ==(Dpi dpi1, Dpi dpi2)
        {
            return dpi1.X == dpi2.X && dpi1.Y == dpi2.Y;
        }

        /// <summary>
        /// オブジェクトが非等価かどうかを判断します。
        /// </summary>
        /// <param name="dpi1">比較対象1</param>
        /// <param name="dpi2">比較対象2</param>
        /// <returns>非等価の場合はtrue。それ以外はfalse。</returns>
        public static bool operator !=(Dpi dpi1, Dpi dpi2)
        {
            return !(dpi1 == dpi2);
        }

        /// <summary>
        /// 指定したオブジェクトが、現在のオブジェクトと等しいかどうかを判断します。
        /// </summary>
        /// <param name="other">指定オブジェクト</param>
        /// <returns>指定したオブジェクトが現在のオブジェクトと等しい場合は true。それ以外の場合は false</returns>
        public bool Equals(Dpi other)
        {
            return this.X == other.X && this.Y == other.Y;
        }

        /// <summary>
        /// 指定したオブジェクトが、現在のオブジェクトと等しいかどうかを判断します。
        /// </summary>
        /// <param name="other">指定オブジェクト</param>
        /// <returns>指定したオブジェクトが現在のオブジェクトと等しい場合は true。それ以外の場合は false</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Dpi && this.Equals((Dpi)obj);
        }

        /// <summary>
        /// ハッシュコードを取得します。
        /// </summary>
        /// <returns>ハッシュコード</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)this.X * 397) ^ (int)this.Y;
            }
        }
    }
}
