﻿using Livet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using WinCap.Interop;
using WinCap.Serialization;
using WinCap.Services;
using WinCap.Views;
using WpfUtility.Lifetime;
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
        /// フックサービス。
        /// </summary>
        internal HookService HookService { get; private set; }

        /// <summary>
        /// ウィンドウサービス。
        /// </summary>
        internal WindowService WindowService { get; private set; }

        /// <summary>
        /// キャプチャサービス。
        /// </summary>
        internal CapturerService CapturerService { get; private set; }

        /// <summary>
        /// アプリケーションアクション。
        /// </summary>
        internal ApplicationAction ApplicationAction { get; private set; }

        /// <summary>
        /// 現在の <see cref="AppDomain"/> の <see cref="Application"/> オブジェクトを取得します。
        /// </summary>
        public static Application Instance => Current as Application;

        /// <summary>
        /// 静的コンストラクタ。
        /// </summary>
        static Application()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => ReportException(sender, args.ExceptionObject as Exception);
        }

        /// <summary>
        /// 起動イベント。
        /// </summary>
        /// <param name="e">イベント引数</param>
        protected override void OnStartup(StartupEventArgs e)
        {
#if !DEBUG
            // 多重起動防止チェック
            var appInstance = new WpfUtility.Desktop.ApplicationInstance().AddTo(this);
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
                this.compositeDisposable.Add(LocalSettingsProvider.Instance.Save);

                // メインウィンドウ表示
                this.MainWindow = new MainWindow();
                if (e.Args.Length > 0 && e.Args[0] == "-UITest")
                {
                    // UIテストの場合、メインウィンドウを表示する
                    this.MainWindow.ShowInTaskbar = true;
                    this.MainWindow.WindowState = WindowState.Normal;
                }
                this.MainWindow.Show();

                this.HookService = new HookService(this.MainWindow).AddTo(this);
                this.WindowService = new WindowService().AddTo(this);
                this.CapturerService = new CapturerService(this.HookService, this.WindowService).AddTo(this);
                this.ApplicationAction = new ApplicationAction(this).AddTo(this);

                // アプリケーション準備
                this.ShowTaskTrayIcon();
                this.ApplicationAction.CreateShortcut();
                if (!this.ApplicationAction.RegisterActions())
                {
                    Task.Run(() => this.ApplicationAction.ShowSettings());
                }

                // 親メソッド呼び出し
                base.OnStartup(e);
            }
#if !DEBUG
            else
            {
                this.Shutdown();
            }
#endif
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
                    new TaskTrayIconItem(PropResources.ContextMenu_RegionCapture, () => this.CapturerService.CaptureSelectionRegion()),
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
        /// <param name="isShutdown">シャットダウンするかどうか</param>
        public static void ReportException(object sender, Exception exception, bool isShutdown = true)
        {
            #region const

            const string messageFormat = @"
===========================================================
ERROR, date = {0}, sender = {1},
{2}
";
            const string path = "error.log";

            #endregion const

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
            if (isShutdown)
            {
                Current.Shutdown();
            }
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

        #endregion IDisposableHolder members
    }
}