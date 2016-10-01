﻿using Livet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using WinCap.Models;
using WinCap.Services;
using WinCap.Util.Lifetime;
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
        /// キャプチャサービス
        /// </summary>
        internal CaptureService CaptureService { get; private set; }

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

                // 設定のロード
                //SettingsHost.Load();
                //compositeDisposable.Add(SettingsHost.Save);

                // 各サービスの初期化
                this.ShowNotifyIcon();
                this.HookService = new HookService().AddTo(this);
                WindowService.Current.AddTo(this);
                this.CaptureService = new CaptureService().AddTo(this);
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
            Uri uri;
            if (!Uri.TryCreate(WinCap.Properties.Settings.Default.IconUri, UriKind.Absolute, out uri)) return;

            var streamResourceInfo = GetResourceStream(uri);
            if (streamResourceInfo == null) return;

            using (var stream = streamResourceInfo.Stream)
            {
                var menus = new List<MenuItem>();
                var captureMenus = new List<MenuItem>()
                {
                    new MenuItem(PropResources.ContextMenu_ScreenCapture, (sender, args) => MessageBox.Show("ScreenCapture")),
                    new MenuItem(PropResources.ContextMenu_ControlCapture, (sender, args) => MessageBox.Show("ControlCapture")),
                    new MenuItem(PropResources.ContextMenu_PageCapture, (sender, args) => MessageBox.Show("PageCaoture"))
                };
                menus.Add(new MenuItem(PropResources.ContextMenu_Capture, captureMenus.ToArray()));
                menus.Add(new MenuItem(PropResources.ContextMenu_Setting, (sender, args) => MessageBox.Show("Setting")));
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
            ShortcutKey scKey = new ShortcutKey(System.Windows.Forms.Keys.PrintScreen, System.Windows.Forms.Keys.LControlKey);
            this.HookService
                .Register(scKey, () => this.CaptureService.CaptureSelectControl())
                .AddTo(this);
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
