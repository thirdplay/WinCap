﻿using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RM.Friendly.WPFStandardControls;
using System.Diagnostics;
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
            this.app = new WindowsAppFriend(Process.Start(@"WinCap.exe", "-UITest"));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            //Process.GetProcessById(this.app.ProcessId).CloseMainWindow();
            this.app.Type<Application>().Current.Shutdown();
        }

        [TestMethod]
        public void TestMethod1()
        {
            dynamic app = this.app.Type<Application>().Current;
            app.ApplicationAction.ShowSettings();
            //Thread.Sleep(1000 * 2);

            var w = new WindowControl(app.SettingsWindow);
            var visualTree = w.VisualTree();
            var scrollDelayTime = new WPFTextBox(visualTree.ByBinding("ScrollDelayTime").Single());
            //var content = new WPFContentControl(w.LogicalTree().ByType("System.Windows.Controls.ContentControl")[0]);
            //var button = new WPFButtonBase(w.LogicalTree().ByType("System.Windows.Controls.Button")[0]);
            //button.EmulateClick();
            //var value = new WPFTabControl(w.LogicalTree().ByType("ItemsControl").Single());

            //MessageBox.Show("Result:" + settingsWindow.ApplicationAction);
            Assert.AreEqual("100", scrollDelayTime.Text);
        }

        [TestMethod]
        public void TestMethod2()
        {
            dynamic app = this.app.Type<Application>().Current;
            app.ApplicationAction.ShowSettings();

            var w = new WindowControl(app.SettingsWindow);
            var logicalTree = w.LogicalTree();
            var listBox = new WPFListBox(logicalTree.ByType("MetroRadiance.UI.Controls.TabView").ByBinding("TabItems").Single());

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
    }
}
