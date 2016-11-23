using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RM.Friendly.WPFStandardControls;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WinCap.Test
{
    [TestClass]
    public class SettingsWindowTest
    {
        WindowsAppFriend app;

        [TestInitialize]
        public void TestInitialize()
        {
#if DEBUG
            var build = "Debug";
#else
            var build = "Release";
#endif
            var exePath = Path.GetFullPath("../../../../WinCap/bin/x86/" + build + "/WinCap.exe");
            this.app = new WindowsAppFriend(Process.Start(exePath, "-ShowSettings"));
            //this.app = new WindowsAppFriend(Process.Start(@"app\WinCap.exe", "-ShowSettings"));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.app.Type<Application>().Current.Shutdown();
        }

        [TestMethod]
        public void TestMethod1()
        {
            dynamic app = this.app.Type<Application>().Current;
            dynamic settingsWindow = app.ApplicationAction.ShowSettings();
            dynamic settingsWindowViewModel = settingsWindow.DataContext;
            dynamic generalViewModel = settingsWindowViewModel.General;
            var w = new WindowControl(settingsWindow);

            var visualTree = w.VisualTree();
            var scrollDelayTime = new WPFTextBox(visualTree.ByBinding("ScrollDelayTime").Single());
            var buttonOk = new WPFButtonBase(settingsWindow._buttonOk);
            scrollDelayTime.EmulateChangeText("a");
            buttonOk.EmulateClick();

            string errorMessage = generalViewModel.GetErrors("ScrollDelayTime")[0];
            //viewModel.General.GetErrors("ScrollDelayTime")[0]
            //var content = new WPFContentControl(w.LogicalTree().ByType("System.Windows.Controls.ContentControl")[0]);
            //var button = new WPFButtonBase(w.LogicalTree().ByType("System.Windows.Controls.Button")[0]);
            //button.EmulateClick();
            //var value = new WPFTabControl(w.LogicalTree().ByType("ItemsControl").Single());

            //MessageBox.Show("Result:" + settingsWindow.ApplicationAction);
            Assert.AreEqual("0以上、1000以下の数値を入力してください。", errorMessage);
        }

        [TestMethod]
        public void TestMethod2()
        {
            dynamic app = this.app.Type<Application>().Current;
            var settingsWindow = app.ApplicationAction.ShowSettings();

            var w = new WindowControl(settingsWindow);
            var logicalTree = w.LogicalTree();
            var listBox = new WPFListBox(logicalTree.ByType("MetroRadiance.UI.Controls.TabView").ByBinding("TabItems").Single());

            Thread.Sleep(1000 * 1);
            listBox.EmulateChangeSelectedIndex(1);
            Thread.Sleep(1000 * 1);
            listBox.EmulateChangeSelectedIndex(2);
            Thread.Sleep(1000 * 1);
            listBox.EmulateChangeSelectedIndex(3);
            Thread.Sleep(1000 * 1);
            listBox.EmulateChangeSelectedIndex(0);
            Thread.Sleep(1000 * 1);

            Assert.AreEqual("100", "100");
        }

        [TestMethod]
        public void TestMethod3()
        {
            dynamic app = this.app.Type<Application>().Current;
            dynamic settingsWindow = app.ApplicationAction.ShowSettings();
            var wc = new WindowControl(settingsWindow);
            var visualTree = wc.VisualTree();
            var logicalTree = wc.LogicalTree();
            var listBox = new WPFListBox(logicalTree.ByType("MetroRadiance.UI.Controls.TabView").ByBinding("TabItems").Single());


            listBox.EmulateChangeSelectedIndex(1);
            Thread.Sleep(1000 * 1);
            var itemControl = new WPFListViewItem(logicalTree.ByType("System.Windows.Controls.ItemsControl").ByBinding("TabItems")[1]);
            //wc.VisualTree().ByType<Button>()[4]
            //var buttonSelection = new WPFButtonBase(itemControl._buttonSelection);
            //buttonSelection.EmulateClick();
            //Thread.Sleep(1000 * 2);
        }
    }
}
