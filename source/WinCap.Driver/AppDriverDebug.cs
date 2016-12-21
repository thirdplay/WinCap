using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace WinCap.Driver
{
    /// <summary>
    /// デバッグ用のアプリケーション操作機能を提供します。
    /// </summary>
    public class AppDriverDebug : IAppDriverCore
    {
        /// <summary>
        /// アプリケーション操作クラス。
        /// </summary>
        private WindowsAppFriend _app;

        /// <summary>
        /// デバッグプロセスが存在するか判定します。
        /// </summary>
        public static bool Exists => GetDebugProcess() != null;

        #region IAppDriverCore members

        /// <summary>
        /// プロセスを取得します。
        /// </summary>
        public Process Process { get; private set; }

        /// <summary>
        /// アプリケーションをアタッチします。
        /// </summary>
        /// <returns>アプリケーション操作クラス</returns>
        public void Attach()
        {
            this.Process = GetDebugProcess();
            this._app = new WindowsAppFriend(this.Process);
        }

        /// <summary>
        /// リソースを解放します。
        /// </summary>
        /// <param name="isContinue">処理を継続するかどうかを示す値</param>
        public void Release(bool isContinue)
        {
            this._app?.Dispose();
            this._app = null;
        }

        /// <summary>
        /// アプリケーションを終了します。
        /// </summary>
        public void EndProcess() { }

        /// <summary>
        /// 設定ウィンドウを表示します。
        /// </summary>
        /// <returns>設定ウィンドウドライバー</returns>
        public SettingsWindowDriver ShowSettingsWindow()
        {
            var appVar = this._app.Type<Application>().Current;
            return new SettingsWindowDriver(appVar.ApplicationAction.ShowSettings());
        }

        #endregion

        /// <summary>
        /// 起動中のデバッグプロセスを取得します。
        /// </summary>
        /// <returns>デバッグプロセス</returns>
        public static Process GetDebugProcess()
        {
            return Process.GetProcessesByName("WinCap.vshost").Where(e => e.MainWindowHandle != IntPtr.Zero).FirstOrDefault();
        }
    }
}
