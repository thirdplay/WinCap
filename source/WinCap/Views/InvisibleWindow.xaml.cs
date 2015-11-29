using MetroRadiance.Controls;

namespace WinCap.Views
{
    /// <summary>
    /// InvisibleWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class InvisibleWindow
    {
        public InvisibleWindow()
        {
            InitializeComponent();
            this.Loaded += (sender, e) => this.Hide();
        }
    }
}
