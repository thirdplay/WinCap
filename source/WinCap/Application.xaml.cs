using Livet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using WinCap.Interop;
using WinCap.Models;
using WinCap.Serialization;
using WinCap.Services;
using WinCap.Util.Lifetime;
using WinCap.Views;
using PropResources = WinCap.Properties.Resources;

namespace WinCap
{
    /// <summary>
    /// Application.xaml の相互作用ロジック
    /// </summary>
    sealed partial class Application : IDisposableHolder
    {
        /// <summary>
        /// 基本CompositeDisposable。
        /// </summary>
        private readonly LivetCompositeDisposable compositeDisposable = new LivetCompositeDisposable();

        /// <summary>
        /// フックサービス
        /// </summary>
        internal HookService HookService { get; private set; }

        /// <summary>
        /// キャプチャサービス
        /// </summary>
        internal CapturerService CapturerService { get; private set; }

        /// <summary>
        /// 設定ウィンドウ
        /// </summary>
        internal SettingsWindow SettingsWindow { get; set; }

        /// <summary>
        /// アプリケーションアクション
        /// </summary>
        internal ApplicationAction ApplicationAction { get; private set; }

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static Application()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => ReportException(sender, args.ExceptionObject as Exception);
        }

        /// <summary>
        /// 起動イベント
        /// </summary>
        /// <param name="e">イベント引数</param>
        protected override void OnStartup(StartupEventArgs e)
        {
#if !DEBUG
            // 多重起動防止チェック
            var appInstance = new Util.Desktop.ApplicationInstance().AddTo(this);
            if (appInstance.IsFirst)
#endif
            {
                this.DispatcherUnhandledException += (sender, args) =>
                {
                    ReportException(sender, args.Exception);
                    args.Handled = true;
                };

                // UIDispatcherの設定
                DispatcherHelper.UIDispatcher = this.Dispatcher;

                // ローカル設定の読み込み
                LocalSettingsProvider.Instance.Load();

                this.HookService = new HookService().AddTo(this);
                this.CapturerService = new CapturerService(this.HookService).AddTo(this);
                this.ApplicationAction = new ApplicationAction(this).AddTo(this);

                // アプリケーション準備
                this.CreateShortcut();
                this.ShowTaskTrayIcon();

                // 親メソッド呼び出し
                base.OnStartup(e);

                // オプションが指定されている場合、設定ウィンドウを表示する
                if (e.Args.Length > 0 && e.Args[0] == "-ShowSettings")
                {
                    this.ApplicationAction.ShowSettings();
                }
            }
#if !DEBUG
            else
            {
                this.Shutdown();
            }
#endif
        }

        /// <summary>
        /// ショートカットを作成します。
        /// </summary>
        public void CreateShortcut()
        {
            var shortcut = new StartupShortcut();
            shortcut.Recreate(Settings.General.IsRegisterInStartup);
            var desktopShortcut = new DesktopShortcut();
            desktopShortcut.Recreate(Settings.General.IsCreateShortcutToDesktop);
        }

        /// <summary>
        /// タスクトレイアイコンを表示します。
        /// </summary>
        public void ShowTaskTrayIcon()
        {
            const string iconUri = "pack://application:,,,/WinCap;component/Assets/app.ico";
            Uri uri;
            if (!Uri.TryCreate(iconUri, UriKind.Absolute, out uri)) { return; }

            var icon = IconHelper.GetIconFromResource(uri);
            var menus = new[]
            {
                new TaskTrayIconItem(PropResources.ContextMenu_Capture, new[]
                {
                    new TaskTrayIconItem(PropResources.ContextMenu_DesktopCapture, () => this.CapturerService.CaptureDesktop()),
                    new TaskTrayIconItem(PropResources.ContextMenu_ControlCapture, () => this.CapturerService.CaptureSelectionControl()),
                    new TaskTrayIconItem(PropResources.ContextMenu_WebPageCapture, () => this.CapturerService.CaptureWebPage()),
                }),
                new TaskTrayIconItem(PropResources.ContextMenu_Settings, () => this.ApplicationAction.ShowSettings()),
                new TaskTrayIconItem(PropResources.ContextMenu_Exit, () => { this.Shutdown(); }),
            };

            var taskTrayIcon = new TaskTrayIcon(icon, menus);
            taskTrayIcon.Show();
            taskTrayIcon.AddTo(this);
        }

        /// <summary>
        /// 終了イベント
        /// </summary>
        /// <param name="e">イベント引数</param>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            this.Dispose();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// 例外報告
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="exception">例外</param>
        private static void ReportException(object sender, Exception exception)
        {
#region const
            const string messageFormat = @"
===========================================================
ERROR, date = {0}, sender = {1},
{2}
";
            const string path = "error.log";
#endregion

            try
            {
                var message = string.Format(messageFormat, DateTimeOffset.Now, sender, exception);

                Debug.WriteLine(message);
                File.AppendAllText(path, message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            // 終了
            Current.Shutdown();
        }

#region IDisposableHolder members
        ICollection<IDisposable> IDisposableHolder.CompositeDisposable => this.compositeDisposable;

        /// <summary>
        /// このインスタンスによって使用されているリソースを全て破棄します。
        /// </summary>
        public void Dispose()
        {
            this.compositeDisposable.Dispose();
        }
#endregion
    }
}
