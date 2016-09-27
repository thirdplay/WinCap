using System;
using WinCap.Properties;

namespace WinCap.ViewModels.Settings
{
    /// <summary>
    /// 出力項目のためのデータを提供します。
    /// </summary>
    public class OutputViewModel : TabItemViewModel
    {
        /// <summary>
        /// タブ名称を取得します。
        /// </summary>
        public override string Name
        {
            get { return Resources.OutputSetting; }
            protected set{throw new NotImplementedException();}
        }
    }
}
