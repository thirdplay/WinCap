using Livet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using WinCap.Serialization;
using WinCap.Services;
using WinCap.Util.Lifetime;

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

                // アプリケーション準備
                var preparation = new ApplicationPreparation(this);
                preparation.CreateShortcut();
                preparation.ShowTaskTrayIcon();
                preparation.RegisterActions();

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
