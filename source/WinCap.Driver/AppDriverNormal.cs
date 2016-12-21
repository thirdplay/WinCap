using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace WinCap.Driver
{
    /// <summary>
    /// 通常のアプリケーション操作機能を提供します。
    /// </summary>
    public class AppDriverNormal : IAppDriverCore
    {
        /// <summary>
        /// ビルドディレクトリ
        /// </summary>
#if DEBUG
        private static string BuildDir { get; } = "Debug";
#else
        private static string BuildDir { get; } = "Release";
#endif

        /// <summary>
        /// 実行ファイルパスを取得します。
        /// </summary>
        private static string ExecutablePath { get; } = Path.GetFullPath("../../../../WinCap/bin/x86/" + BuildDir + "/WinCap.exe");

        /// <summary>
        /// アプリケーション操作クラス。
        /// </summary>
        private WindowsAppFriend _app;

        /// <summary>
        /// タイムアウト検出クラス。
        /// </summary>
        private TimeoutDetector _detector;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public AppDriverNormal()
        {
        }

        #region IAppDriverCore members
        /// <summary>
        /// プロセスを取得します。
        /// </summary>
        public Process Process { get; private set; }

        /// <summary>
        /// アプリケーションをアタッチします。
        /// </summary>
        public void Attach()
        {
            if (this.Process == null)
            {
                this.Process = Process.Start(ExecutablePath, "-ShowSettings");
                this._app = new WindowsAppFriend(this.Process);
                this.ShowSettingsWindow().Close();

                dynamic settings = this._app.Type("WinCap.Serialization.Settings");
                settings.Reset();
            }
            this._detector = new TimeoutDetector(1000 * 60 * 5);
            this._detector.Timedout += (sender, e) =>
            {
                this._app.Type<Application>().Current.Shutdown();
            };
        }

        /// <summary>
        /// リソースを解放します。
        /// </summary>
        /// <param name="isContinue">処理を継続するかどうかを示す値</param>
        public void Release(bool isContinue)
        {
            if (isContinue)
            {
                this._detector.Finish();
            }
            else
            {
                this.EndProcess();
            }
        }

        /// <summary>
        /// アプリケーションを終了します。
        /// </summary>
        public void EndProcess()
        {
            this._detector?.Finish();
            this._detector = null;
            this._app?.Type<Application>().Current.Shutdown();
            this._app = null;
        }

        /// <summary>
        /// 設定ウィンドウを表示します。
        /// </summary>
        /// <returns>設定ウィンドウドライバー</returns>
        public SettingsWindowDriver ShowSettingsWindow()
        {
            var appVar = this._app.Type<Application>().Current;
            return new SettingsWindowDriver(new WindowControl(appVar.ApplicationAction.ShowSettings()));
        }
        #endregion
    }
}
