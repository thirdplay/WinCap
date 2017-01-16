namespace WinCap.Models
{
    /// <summary>
    /// ファイル名パターンを表します。
    /// </summary>
    public class FileNamePattern
    {
        /// <summary>
        /// 年
        /// </summary>
        public const string Year = "%y";

        /// <summary>
        /// 月
        /// </summary>
        public const string Month = "%m";

        /// <summary>
        /// 日
        /// </summary>
        public const string Day = "%d";

        /// <summary>
        /// 時
        /// </summary>
        public const string Hour = "%h";

        /// <summary>
        /// 分
        /// </summary>
        public const string Minute = "%n";

        /// <summary>
        /// 秒
        /// </summary>
        public const string Second = "%s";

        /// <summary>
        /// 連番
        /// </summary>
        public const string Sequence = "%c";
    }
}
