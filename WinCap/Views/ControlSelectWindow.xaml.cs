using Livet;
using System;
using System.Collections.Generic;
using WinCap.Utilities.Lifetime;

namespace WinCap.Views
{
    /// <summary>
    /// ControlSelectWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ControlSelectWindow
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ControlSelectWindow()
        {
            InitializeComponent();
            this.MouseDown += Button_Click;
        }

        // TODO:実装時に下記をビヘイビアへ移動する
        // ※EventTrigger
        public event EventHandler Selected = null;
        private void Button_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Selected?.Invoke(sender, e);
        }
    }
}
