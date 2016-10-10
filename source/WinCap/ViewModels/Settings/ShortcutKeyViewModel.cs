using System;
using WinCap.Properties;

namespace WinCap.ViewModels.Settings
{
    /// <summary>
    /// ショートカットキー設定のためのデータを提供します。
    /// </summary>
    public class ShortcutKeyViewModel : TabItemViewModel
    {
        /// <summary>
        /// タブ名称を取得します。
        /// </summary>
        public override string Name
        {
            get { return Resources.HotkeySetting; }
            protected set{throw new NotImplementedException();}
        }
    }
}
