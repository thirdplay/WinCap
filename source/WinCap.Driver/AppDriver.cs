using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace WinCap.Driver
{
    /// <summary>
    /// 対象アプリケーションを操作する機能を提供します。
    /// </summary>
    public class AppDriver
    {
        /// <summary>
        /// Windowsアプリケーション操作クラス。
        /// </summary>
        private WindowsAppFriend _app;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public AppDriver()
        {
#if DEBUG
            var build = "Debug";
#else
            var build = "Release";
#endif
            var exePath = Path.GetFullPath("../../../../WinCap/bin/x86/" + build + "/WinCap.exe");
            this._app = new WindowsAppFriend(Process.Start(exePath, "-ShowSettings"));
        }

        /// <summary>
        /// リソースを解放します。
        /// </summary>
        public void Release()
        {
            //this._app.Dispose();
            this._app.Type<Application>().Current.Shutdown();
        }
    }
}
