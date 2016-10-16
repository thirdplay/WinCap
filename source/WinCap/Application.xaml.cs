using Livet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using WinCap.Models;
using WinCap.Serialization;
using WinCap.Services;
using WinCap.Util.Lifetime;
using WinCap.Views;
using MenuItem = System.Windows.Forms.MenuItem;
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
        /// 通知アイコン
        /// </summary>
        internal System.Windows.Forms.NotifyIcon _notifyIcon { get; private set; }

        /// <summary>
        /// フックサービス
        /// </summary>
        internal HookService HookService { get; private set; }

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
            // 多重起動防止チェック
            var appInstance = new Util.Desktop.ApplicationInstance().AddTo(this);
            if (appInstance.IsFirst)
            {
                //Console.WriteLine("Total Memory = {0} KB", GC.GetTotalMemory(true) / 1024);
                this.DispatcherUnhandledException += (sender, args) =>
                {
                    ReportException(sender, args.Exception);
                    args.Handled = true;
                };

                // UIDispatcherの設定
                DispatcherHelper.UIDispatcher = this.Dispatcher;

                // ローカル設定の読み込み
                LocalSettingsProvider.Instance.LoadAsync().Wait();
                LocalSettingsProvider.Instance.AddTo(this);

                // 各サービスの初期化
                this.ShowNotifyIcon();
                this.HookService = new HookService().AddTo(this);
                WindowService.Current.AddTo(this);
                CapturableService.Current.AddTo(this);
                this.RegisterActions();

                // 親メソッド呼び出し
                base.OnStartup(e);
            }
            else
            {
                this.Shutdown();
            }
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
        /// 通知アイコンを表示します。
        /// </summary>
        private void ShowNotifyIcon()
        {
            const string iconUri = "pack://application:,,,/WinCap;component/Assets/app.ico";
            Uri uri;
            if (!Uri.TryCreate(iconUri, UriKind.Absolute, out uri)) return;

            var streamResourceInfo = GetResourceStream(uri);
            if (streamResourceInfo == null) return;

            using (var stream = streamResourceInfo.Stream)
            {
                var menus = new List<MenuItem>();
                var captureMenus = new List<MenuItem>()
                {
                    new MenuItem(PropResources.ContextMenu_DesktopCapture, (sender, args) => CapturableService.Current.CaptureDesktop()),
                    new MenuItem(PropResources.ContextMenu_ControlCapture, (sender, args) => CapturableService.Current.CaptureSelectionControl()),
                    new MenuItem(PropResources.ContextMenu_WebPageCapture, (sender, args) => CapturableService.Current.CaptureWebPage())
                };
                menus.Add(new MenuItem(PropResources.ContextMenu_Capture, captureMenus.ToArray()));
                menus.Add(new MenuItem(PropResources.ContextMenu_Settings, (sender, args) => ShowSettings()));
                menus.Add(new MenuItem(PropResources.ContextMenu_Exit, (sender, args) => this.Shutdown()));
                this._notifyIcon = new System.Windows.Forms.NotifyIcon
                {
                    Text = ProductInfo.Title,
                    Icon = new System.Drawing.Icon(stream),
                    Visible = true,
                    ContextMenu = new System.Windows.Forms.ContextMenu(menus.ToArray()),
                };
                this._notifyIcon.AddTo(this);
            }
        }

        /// <summary>
        /// アクションを登録します。
        /// </summary>
        private void RegisterActions()
        {
            var settings = Settings.ShortcutKey;

            this.HookService
                .Register(settings.FullScreen.ToShortcutKey(), () => CapturableService.Current.CaptureDesktop())
                .AddTo(this);

            this.HookService
                .Register(settings.ActiveControl.ToShortcutKey(), () => CapturableService.Current.CaptureActiveControl())
                .AddTo(this);

            this.HookService
                .Register(settings.SelectionControl.ToShortcutKey(), () => CapturableService.Current.CaptureSelectionControl())
                .AddTo(this);

            this.HookService
                .Register(settings.WebPage.ToShortcutKey(), () => CapturableService.Current.CaptureWebPage())
                .AddTo(this);
        }

        /// <summary>
        /// 設定ウィンドウを表示します。
        /// </summary>
        private void ShowSettings()
        {
            var window = WindowService.Current.GetSettingsWindow();
            if (window.IsLoaded)
            {
                window.Activate();
                return;
            }
            using (this.HookService.Suspend())
            {
                window.ShowDialog();
            }
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
