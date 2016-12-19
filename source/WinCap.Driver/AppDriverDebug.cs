using Codeer.Friendly.Windows;
using System;
using System.Diagnostics;
using System.Linq;

namespace WinCap.Driver
{
    /// <summary>
    /// デバッグ用のアプリケーション操作機能を提供します。
    /// </summary>
    public class AppDriverDebug : IAppDriverCore
    {
        /// <summary>
        /// デバッグプロセスが存在するか判定します。
        /// </summary>
        public static bool Exists { get { return GetDebugProcess() != null; } }

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
        public void SetTimeout(int time) { }

        /// <summary>
        /// アプリケーションをアタッチします。
        /// </summary>
        /// <returns>アプリケーション操作クラス</returns>
        public WindowsAppFriend Attach()
        {
            this.Process = GetDebugProcess();
            this.App = new WindowsAppFriend(this.Process);
            return this.App;
        }

        /// <summary>
        /// リソースを解放します。
        /// </summary>
        /// <param name="isContinue">処理を継続するかどうかを示す値</param>
        public void Release(bool isContinue)
        {
            this.App.Dispose();
        }

        /// <summary>
        /// アプリケーションを終了します。
        /// </summary>
        public void EndProcess() { }
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
