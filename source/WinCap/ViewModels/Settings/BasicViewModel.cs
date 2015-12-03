﻿using System;
using WinCap.Properties;

namespace WinCap.ViewModels.Settings
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
            get { return Resources.BasicSetting; }
            protected set{throw new NotImplementedException();}
        }
    }
}