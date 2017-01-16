using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WinCap.Test
{
    /// <summary>
    /// 設定ウィンドウのUIテスト。
    /// </summary>
    [TestClass]
    public class SettingsWindowTest : TestBase<SettingsWindowTest>
    {
        /// <summary>
        /// テストクラスを初期化します。
        /// </summary>
        /// <param name="c">テストコンテキスト</param>
        [ClassInitialize]
        public static void ClassInitialize(TestContext c)
        {
            NotifyClassInitialize();
        }

        /// <summary>
        /// テストクラスを終了化します。
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
            NotifyClassCleanup();
        }

        /// <summary>
        /// テスト開始処理。
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.NotifyTestInitialize();
        }

        /// <summary>
        /// テスト終了処理。
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            this.NotifyTestCleanup();
        }

        /// <summary>
        /// スクロール遅延時間のテスト。
        /// </summary>
        [TestMethod]
        public void TestScrollDelayTime()
        {
            var settingsWindow = App.ShowSettingsWindow();
            var general = settingsWindow.General;
            var buttonOk = settingsWindow.ButtonOk;

            general.ScrollDelayTime.EmulateChangeText("200");
            buttonOk.EmulateClick();
            string errorMessage = settingsWindow.General.GetError("ScrollDelayTime");

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual("200", general.ScrollDelayTime.Text);
        }

        /// <summary>
        /// キャプチャ遅延時間のテスト。
        /// </summary>
        [TestMethod]
        public void TestCaptureDelayTime()
        {
            var settingsWindow = App.ShowSettingsWindow();
            var general = settingsWindow.General;
            var buttonOk = settingsWindow.ButtonOk;

            general.CaptureDelayTime.EmulateChangeText("300");
            buttonOk.EmulateClick();
            string errorMessage = settingsWindow.General.GetError("CaptureDelayTime");

            Assert.IsTrue(string.IsNullOrEmpty(errorMessage));
            Assert.AreEqual("300", general.CaptureDelayTime.Text);
        }

        /// <summary>
        /// 全般タブのエラーパラメータを表します。
        /// </summary>
        class GeneralErrorParam
        {
            public string ScrollDelayTime { get; set; }
            public string CaptureDelayTime { get; set; }
            public string PropertyName { get; set; }
            public string Message { get; set; }
        }

        /// <summary>
        /// 全般タブの入力エラーテスト。
        /// </summary>
        [TestMethod]
        [DataSource("System.Data.OleDB",
            @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source=Params.xlsx; Extended Properties='Excel 12.0;HDR=yes';",
            "TestGeneralError$",
            DataAccessMethod.Sequential
        )]
        public void TestGeneralError()
        {
            var param = GetParam<GeneralErrorParam>();
            var settingsWindow = App.ShowSettingsWindow();
            var general = settingsWindow.General;
            var buttonOk = settingsWindow.ButtonOk;

            general.ScrollDelayTime.EmulateChangeText(param.ScrollDelayTime);
            general.CaptureDelayTime.EmulateChangeText(param.CaptureDelayTime);
            buttonOk.EmulateClick();
            string errorMessage = settingsWindow.General.GetError(param.PropertyName);
            Assert.AreEqual(param.Message, errorMessage);
        }

        /// <summary>
        /// 出力タブのエラーパラメータを表します。
        /// </summary>
        class OutputErrorParam
        {
            public string OutputFolder { get; set; }
            public bool IsAutoSaveImage { get; set; }
            public string PropertyName { get; set; }
            public string Message { get; set; }
        }

        /// <summary>
        /// 出力タブの入力エラーテスト。
        /// </summary>
        [TestMethod]
        [DataSource("System.Data.OleDB",
            @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source=Params.xlsx; Extended Properties='Excel 12.0;HDR=yes';",
            "TestOutputError$",
            DataAccessMethod.Sequential
        )]
        public void TestOutputError()
        {
            var param = GetParam<OutputErrorParam>();
            var settingsWindow = App.ShowSettingsWindow();
            var output = settingsWindow.Output;
            var buttonOk = settingsWindow.ButtonOk;

            output.OutputFolder.EmulateChangeText(param.OutputFolder);
            output.IsAutoSaveImage.EmulateCheck(param.IsAutoSaveImage);
            buttonOk.EmulateClick();
            string errorMessage = settingsWindow.Output.GetError(param.PropertyName);
            Assert.AreEqual(param.Message, errorMessage);
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
