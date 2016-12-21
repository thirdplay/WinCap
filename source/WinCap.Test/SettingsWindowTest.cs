using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using WinCap.Driver;

namespace WinCap.Test
{
    /// <summary>
    /// 設定ウィンドウのUIテスト。
    /// </summary>
    [TestClass]
    public class SettingsWindowTest
    {
        /// <summary>
        /// アプリケーションドライバー。
        /// </summary>
        private static AppDriver _app;

        /// <summary>
        /// テスト結果。
        /// </summary>
        private static Dictionary<string, bool> _tests;

        /// <summary>
        /// テストコンテキストを取得、設定します。
        /// </summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        /// テストクラスを初期化します。
        /// </summary>
        /// <param name="c">テストコンテキスト</param>
        [ClassInitialize]
        public static void ClassInitialize(TestContext c)
        {
            _app = new AppDriver();
            _tests = typeof(SettingsWindowTest).GetMethods().Where(e => 0 < e.GetCustomAttributes(typeof(TestMethodAttribute), true).Length).ToDictionary(e => e.Name, e => true);
        }

        /// <summary>
        /// テストクラスを終了化します。
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
            _app.EndProcess();
        }

        /// <summary>
        /// テスト開始処理。
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            _app.Attach();
        }

        /// <summary>
        /// テスト終了処理。
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            if (TestContext.DataRow == null ||
                ReferenceEquals(TestContext.DataRow, TestContext.DataRow.Table.Rows[TestContext.DataRow.Table.Rows.Count - 1]))
            {
                _tests.Remove(TestContext.TestName);
            }
            _app.Release(TestContext.CurrentTestOutcome == UnitTestOutcome.Passed && 0 < _tests.Count);
        }

        /// <summary>
        /// スクロール遅延時間のテスト。
        /// </summary>
        [TestMethod]
        public void TestScrollDelayTime()
        {
            var settingsWindow = _app.ShowSettingsWindow();
            var general = settingsWindow.General;
            var buttonOk = settingsWindow.ButtonOk;

            general.ScrollDelayTime.EmulateChangeText("");
            buttonOk.EmulateClick();
            string errorMessage = settingsWindow.General.GetError("ScrollDelayTime");
            Assert.AreEqual("必須項目です。", errorMessage);

            general.ScrollDelayTime.EmulateChangeText("a");
            buttonOk.EmulateClick();
            errorMessage = settingsWindow.General.GetError("ScrollDelayTime");
            Assert.AreEqual("0以上、1000以下の数値を入力してください。", errorMessage);
        }

        /// <summary>
        /// キャプチャ遅延時間のテスト。
        /// </summary>
        [TestMethod]
        public void TestCaptureDelayTime()
        {
            var settingsWindow = _app.ShowSettingsWindow();
            var general = settingsWindow.General;
            var buttonOk = settingsWindow.ButtonOk;

            general.CaptureDelayTime.EmulateChangeText("");
            buttonOk.EmulateClick();
            string errorMessage = settingsWindow.General.GetError("CaptureDelayTime");
            Assert.AreEqual("必須項目です。", errorMessage);

            general.CaptureDelayTime.EmulateChangeText("a");
            buttonOk.EmulateClick();
            errorMessage = settingsWindow.General.GetError("CaptureDelayTime");
            Assert.AreEqual("0以上、10000以下の数値を入力してください。", errorMessage);
        }

        ///// <summary>
        ///// 全般タブの入力エラーテスト。
        ///// </summary>
        //[TestMethod]
        //public void TestGeneralError()
        //{
        //}
        
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
