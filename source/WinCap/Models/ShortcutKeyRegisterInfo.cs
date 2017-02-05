using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCap.Models
{
    /// <summary>
    /// ショートカットキーの登録情報を表します。
    /// </summary>
    public class ShortcutKeyRegisterInfo
    {
        /// <summary>
        /// ショートカットキーの名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ショートカットキー
        /// </summary>
        public ShortcutKey ShortcutKey { get; set; }

        /// <summary>
        /// ショートカットキーに押下時に呼ぶアクション
        /// </summary>
        public Action Action { get; set; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="name">ショートカットキーの名称</param>
        /// <param name="shortcutKey">ショートカットキー</param>
        /// <param name="action">ショートカットキーに押下時に呼ぶアクション</param>
        public ShortcutKeyRegisterInfo(string name, ShortcutKey shortcutKey, Action action)
        {
            this.Name = name;
            this.ShortcutKey = shortcutKey;
            this.Action = action;
        }
    }
}
