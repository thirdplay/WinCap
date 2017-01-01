using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCap.Driver
{
    /// <summary>
    /// タブ項目の種類を表します。
    /// </summary>
    public enum TabItem : int
    {
        /// <summary>
        /// 全般
        /// </summary>
        General,

        /// <summary>
        /// 出力
        /// </summary>
        Output,

        /// <summary>
        /// ショートカット
        /// </summary>
        ShortcutKey,

        /// <summary>
        /// バージョン情報
        /// </summary>
        Version,
    }
}
