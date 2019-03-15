using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCap.Models
{
    /// <summary>
    /// 選択した領域のModel。
    /// </summary>
    public class RegionSelectedModel
    {
        /// <summary>
        /// 選択領域
        /// </summary>
        public ReactiveProperty<Rectangle> SelectedRegion { get; }
    }
}
