using System;
using MetroRadiance.Controls;

namespace WinCap.Views
{
    /// <summary>
    /// SelectWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectWindow
    {
        public SelectWindow()
        {
            InitializeComponent();
        }

        // TODO:実装時に下記をビヘイビアへ移動する
        // ※EventTrigger
        public event EventHandler Selected = null;
        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Selected?.Invoke(sender, e);
        }
    }
}
