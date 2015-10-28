using System.Windows;
using WinCap.Components;

namespace WinCap
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 通知アイコン
        /// </summary>
        private NotifyIconWrapper notifyIcon;

        /// <summary>
        /// 起動イベント
        /// </summary>
        /// <param name="e">イベント引数</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            //this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            this.notifyIcon = new NotifyIconWrapper(WinCap.Properties.Settings.Default.IconUri);

            base.OnStartup(e);
        }

        /// <summary>
        /// 終了イベント
        /// </summary>
        /// <param name="e">イベント引数</param>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            this.notifyIcon.Dispose();
        }
    }
}
