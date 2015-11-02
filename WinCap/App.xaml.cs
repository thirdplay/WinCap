using Livet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using WinCap.Utilities.Lifetime;
using WinCap.Services;
using WinCap.Views;

namespace WinCap
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    sealed partial class App : Application, IDisposableHolder
    {
        /// <summary>
        /// 基本CompositeDisposable。
        /// </summary>
        private readonly LivetCompositeDisposable compositeDisposable = new LivetCompositeDisposable();

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static App()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => ReportException(sender, args.ExceptionObject as Exception);
        }

        /// <summary>
        /// 起動イベント
        /// </summary>
        /// <param name="e">イベント引数</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            //Console.WriteLine("Total Memory = {0} KB", GC.GetTotalMemory(true) / 1024);

            // TODO:多重起動防止チェック

            // 各サービスの初期化
            CaptureService.Current.AddTo(this).Initialize();
            ResidentIconService.Current.AddTo(this).Initialize();
            WindowService.Current.AddTo(this).Initialize();

            // メインウィンドウ生成
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            this.MainWindow = WindowService.Current.GetMainWindow();

            // UnhandledExceptionイベント登録
            this.DispatcherUnhandledException += (sender, args) =>
            {
                ReportException(sender, args.Exception);
                args.Handled = true;
            };

            base.OnStartup(e);
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
            Application.Current.Shutdown();
        }

        #region IDisposableHolder members
        ICollection<IDisposable> IDisposableHolder.CompositeDisposable => this.compositeDisposable;

        /// <summary>
        /// このインスタンスによって使用されているリソースを全て破棄する。
        /// </summary>
        public void Dispose()
        {
            this.compositeDisposable.Dispose();
        }
        #endregion
    }
}
