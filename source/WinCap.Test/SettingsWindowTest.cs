using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RM.Friendly.WPFStandardControls;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WinCap.Driver;

namespace WinCap.Test
{
    [TestClass]
    public class SettingsWindowTest
    {
        AppDriver _appDriver;

        [TestInitialize]
        public void TestInitialize()
        {
            this._appDriver = new AppDriver();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this._appDriver.Release();
        }

        [TestMethod]
        public void TestScrollDelayTime()
        {
            var settingsWindow = this._appDriver.ShowSettingsWindow();

            settingsWindow.General.ScrollDelayTime.EmulateChangeText("a");
            settingsWindow.ButtonOk.EmulateClick();

            string errorMessage = settingsWindow.General.GetError("ScrollDelayTime");

            Assert.AreEqual("0以上、1000以下の数値を入力してください。", errorMessage);
        }

        //[TestMethod]
        //public void TestMethod2()
        //{
        //    dynamic app = this.app.Type<Application>().Current;
        //    var settingsWindow = app.ApplicationAction.ShowSettings();

        //    var w = new WindowControl(settingsWindow);
        //    var logicalTree = w.LogicalTree();
        //    var listBox = new WPFListBox(logicalTree.ByType("MetroRadiance.UI.Controls.TabView").ByBinding("TabItems").Single());

        //    Thread.Sleep(1000 * 1);
        //    listBox.EmulateChangeSelectedIndex(1);
        //    Thread.Sleep(1000 * 1);
        //    listBox.EmulateChangeSelectedIndex(2);
        //    Thread.Sleep(1000 * 1);
        //    listBox.EmulateChangeSelectedIndex(3);
        //    Thread.Sleep(1000 * 1);
        //    listBox.EmulateChangeSelectedIndex(0);
        //    Thread.Sleep(1000 * 1);

        //    Assert.AreEqual("100", "100");
        //}

        //[TestMethod]
        //public void TestMethod3()
        //{
        //    dynamic app = this.app.Type<Application>().Current;
        //    dynamic settingsWindow = app.ApplicationAction.ShowSettings();
        //    var wc = new WindowControl(settingsWindow);
        //    var visualTree = wc.VisualTree();
        //    var logicalTree = wc.LogicalTree();
        //    var listBox = new WPFListBox(logicalTree.ByType("MetroRadiance.UI.Controls.TabView").ByBinding("TabItems").Single());


        //    listBox.EmulateChangeSelectedIndex(1);
        //    Thread.Sleep(1000 * 1);
        //    var itemControl = new WPFListViewItem(logicalTree.ByType("System.Windows.Controls.ItemsControl").ByBinding("TabItems")[1]);
        //    //var buttonSelection = new WPFButtonBase(itemControl._buttonSelection);
        //    //buttonSelection.EmulateClick();
        //    //Thread.Sleep(1000 * 2);
        //    //Thread.Sleep(1000 * 2);
        //    Assert.AreEqual("100", "100");
        //}
    }
}
