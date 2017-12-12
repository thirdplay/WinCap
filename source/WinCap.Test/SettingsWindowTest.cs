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
            var errorMessage = general.ViewModel.GetError("ScrollDelayTime");

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
            var errorMessage = general.ViewModel.GetError("CaptureDelayTime");

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

            var errorMessage = general.ViewModel.GetError(param.PropertyName);
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

            var errorMessage = output.ViewModel.GetError(param.PropertyName);
            Assert.AreEqual(param.Message, errorMessage);
        }

        /// <summary>
        /// ショートカットキータブのエラーパラメータを表します。
        /// </summary>
        class ShortcutKeyErrorParam
        {
            public int[] FullScreen { get; set; }
            public int[] ActiveControl { get; set; }
            public int[] SelectionControl { get; set; }
            public int[] WebPage { get; set; }
            public string[] PropertyNames { get; set; }
            public string Message { get; set; }
        }

        /// <summary>
        /// ショートカットキータブの入力エラーテスト。
        /// </summary>
        [TestMethod]
        [DataSource("System.Data.OleDB",
            @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source=Params.xlsx; Extended Properties='Excel 12.0;HDR=yes';",
            "TestShortcutKeyError$",
            DataAccessMethod.Sequential
        )]
        public void TestShortcutKeyError()
        {
            var param = GetParam<ShortcutKeyErrorParam>();
            var settingsWindow = App.ShowSettingsWindow();
            var shortcutKey = settingsWindow.ShortcutKey;
            var viewModel = shortcutKey.ViewModel;
            var buttonOk = settingsWindow.ButtonOk;

            shortcutKey.FullScreen.EmulateChangeCurrent(param.FullScreen);
            shortcutKey.ActiveControl.EmulateChangeCurrent(param.ActiveControl);
            shortcutKey.SelectionControl.EmulateChangeCurrent(param.SelectionControl);
            shortcutKey.WebPage.EmulateChangeCurrent(param.WebPage);
            buttonOk.EmulateClick();

            foreach(var propertyName in param.PropertyNames)
            {
                Assert.AreEqual(param.Message, viewModel.GetError(propertyName));
            }
        }
    }
}
