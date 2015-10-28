using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;

namespace WinCap.Components
{
    /// <summary>
    /// 通知アイコンのラッパークラス
    /// </summary>
    /// <remarks>汎用化するか要検討</remarks>
    public partial class NotifyIconWrapper : Component
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="iconUri">アイコンURI</param>
        public NotifyIconWrapper(string iconUri)
        {
            InitializeComponent();

            // タスクトレイ用のアイコンを設定
            Stream iconStream = Application.GetResourceStream(new Uri(iconUri)).Stream;
            this.notifyIcon.Icon = new Icon(iconStream);
        }
    }
}
