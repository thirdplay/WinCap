using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace WinCap.Driver
{
    /// <summary>
    /// アプリケーションを操作する機能を提供します。
    /// </summary>
    public class AppDriver
    {
        /// <summary>
        /// ビルドディレクトリ
        /// </summary>
#if DEBUG
        private const string BuildDir = "Debug";
#else
        private const string BuildDir = "Release";
#endif

        /// <summary>
        /// 実行ファイルパスを取得します。
        /// </summary>
        private static string ExecutablePath { get; } = Path.GetFullPath("../../../../WinCap/bin/x86/" + BuildDir + "/WinCap.exe");

        /// <summary>
        /// アプリケーション操作クラス。
        /// </summary>
        private WindowsAppFriend app;

        /// <summary>
        /// タイムアウト検出クラス。
        /// </summary>
        private TimeoutDetector detector;

        /// <summary>
        /// プロセスを取得します。
        /// </summary>
        public Process Process { get; private set; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public AppDriver()
        {
        }

        /// <summary>
        /// アプリケーションをアタッチします。
        /// </summary>
        public void Attach()
        {
            if (this.Process == null)
            {
                this.Process = Process.Start(ExecutablePath, "-UITest");
                this.app = new WindowsAppFriend(this.Process);

                // アプリケーション設定をリセットする
                dynamic provider = this.app.Type("WinCap.Serialization.LocalSettingsProvider");
                provider.Instance.Reset();
            }
            this.detector = new TimeoutDetector(1000 * 60 * 5);
            this.detector.Timedout += (sender, e) =>
            {
                this.Shutdown();
            };
            this.InitApp();
        }

        /// <summary>
        /// リソースを解放します。
        /// </summary>
        /// <param name="isContinue">処理を継続するかどうかを示す値</param>
        public void Release(bool isContinue)
        {
            if (isContinue)
            {
                this.detector.Finish();
            }
            else
            {
                this.EndProcess();
            }
        }

        /// <summary>
        /// アプリケーションを初期化します。
        /// </summary>
        private void InitApp()
        {
        }

        /// <summary>
        /// アプリケーションを終了します。
        /// </summary>
        public void EndProcess()
        {
            if (this.detector != null)
            {
                this.detector.Finish();
                this.detector = null;

                // ショートカット作成を無効にする
                dynamic settings = this.app.Type("WinCap.Serialization.Settings");
                settings.General.IsRegisterInStartup.Value = false;
                settings.General.IsCreateShortcutToDesktop.Value = false;
                dynamic provider = this.app.Type("WinCap.Serialization.LocalSettingsProvider");
                provider.Instance.Save();

                this.Shutdown();
            }
        }

        /// <summary>
        /// 設定ウィンドウを表示します。
        /// </summary>
        /// <returns>設定ウィンドウドライバー</returns>
        public SettingsWindowDriver ShowSettingsWindow()
        {
            dynamic appVar = this.app.Type<Application>().Current;
            return new SettingsWindowDriver(new WindowControl(appVar.ApplicationAction.ShowSettings()));
        }

        /// <summary>
        /// アプリケーションをシャットダウンします。
        /// </summary>
        private void Shutdown()
        {
            this.app?.Type<Application>().Current.Shutdown();
            this.app = null;
        }
    }
}
