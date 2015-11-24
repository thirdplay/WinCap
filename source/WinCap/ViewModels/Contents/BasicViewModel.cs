using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinCap.Properties;

namespace WinCap.ViewModels.Contents
{
    /// <summary>
    /// 基本項目のためのデータを提供します。
    /// </summary>
    public class BasicViewModel : TabItemViewModel
    {
        /// <summary>
        /// タブ名称を取得します。
        /// </summary>
        public override string Name
        {
            get { return Resources.Basic; }
            protected set{throw new NotImplementedException();}
        }
    }
}
