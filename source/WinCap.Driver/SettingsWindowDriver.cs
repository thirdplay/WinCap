using Codeer.Friendly.Windows.Grasp;
using RM.Friendly.WPFStandardControls;

namespace WinCap.Driver
{
    /// <summary>
    /// 設定ウィンドウを操作する機能を提供します。
    /// </summary>
    public class SettingsWindowDriver
    {
        /// <summary>
        /// ウィンドウコントロールを取得します。
        /// </summary>
        public WindowControl Window { get; private set; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="windowControl">ウィンドウコントロール</param>
        public SettingsWindowDriver(WindowControl windowControl)
        {
            var visualTree = windowControl.VisualTree();
            this.Window = windowControl;
        }
    }

    /// <summary>
    /// 設定ウィンドウの全般タブを操作する機能を提供します。
    /// </summary>
    public class General
    {
        /// <summary>
        /// ウィンドウコントロールを取得します。
        /// </summary>
        public WindowControl Window { get; private set; }

        /// <summary>
        /// スクロールの遅延時間を取得します。
        /// </summary>
        public WPFTextBox ScrollDelayTime { get; private set; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="windowControl">ウィンドウコントロール</param>
        public General(WindowControl windowControl)
        {
            var visualTree = windowControl.VisualTree();
            var logicalTree = windowControl.LogicalTree();

            this.Window = windowControl;
            var listBox = new WPFListBox(logicalTree.ByType("MetroRadiance.UI.Controls.TabView").ByBinding("TabItems").Single());

            listBox.EmulateChangeSelectedIndex(1);
            this.ScrollDelayTime = new WPFTextBox(visualTree.ByBinding("ScrollDelayTime").Single());
        }
    }
}
