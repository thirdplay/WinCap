using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
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
        /// タイムアウト検出クラス。
        /// </summary>
        private TimeoutDetector _detector;

        #region IAppDriverCore members
        /// <summary>
        /// プロセスを取得します。
        /// </summary>
        public Process Process { get; private set; }

        /// <summary>
        /// アプリケーション操作クラスを取得します。
        /// </summary>
        public WindowsAppFriend App { get; private set; }

        /// <summary>
        /// タイムアウト時間を設定します。
        /// </summary>
        /// <param name="time">タイムアウト時間</param>
        public void SetTimeout(int time)
        {
            this._detector.Timeout = time;
        }

        /// <summary>
        /// アプリケーションをアタッチします。
        /// </summary>
        /// <returns>アプリケーション操作クラス</returns>
        public WindowsAppFriend Attach()
        {
            if (this.Process == null)
            {
                this.Process = Process.Start(ExecutablePath, "-ShowSettings");
                this.App = new WindowsAppFriend(this.Process);
                //dynamic settings = this._app.Type("WinCap.Serialization.Settings");
            }
            this._detector = new TimeoutDetector(1000 * 60 * 5);
            this._detector.Timedout += (sender, e) =>
            {
                this.App.Type<Application>().Current.Shutdown();
            };

            return this.App;
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
            this.App?.Type<Application>().Current.Shutdown();
            this.App = null;
        }
        #endregion
    }
}
