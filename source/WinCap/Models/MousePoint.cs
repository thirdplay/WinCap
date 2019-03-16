using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Drawing;
using System.Reactive.Disposables;

namespace WinCap.Models
{
    /// <summary>
    /// マウス座標を保持するModel。
    /// </summary>
    public class MousePoint
    {
        /// <summary>
        /// 現在のマウス座標
        /// </summary>
        public ReactiveProperty<Point> CurrentPoint { get; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public MousePoint()
        {
        }
    }
}
