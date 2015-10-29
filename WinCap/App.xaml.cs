using Livet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows;
using WinCap.Components;
using WinCap.Lifetime;
using WinCap.Properties;
using WinCap.Services;
using WinCap.Views;

namespace WinCap
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application, IDisposableHolder
    {
        /// <summary>
        /// 基本CompositeDisposable。
        /// </summary>
        protected readonly LivetCompositeDisposable compositeDisposable = new LivetCompositeDisposable();

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
            // TODO:多重起動防止チェック

            // 各サービスの初期化
            //this.residentIcon = new ResidentIcon();
            ResidentIconService.Current.AddTo(this).Initialize();

            // メインウィンドウ生成
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            this.MainWindow = new MainWindow();

            // UnhandledExceptionイベント登録
            this.DispatcherUnhandledException += (sender, args) =>
            {
                ReportException(sender, args.Exception);
                args.Handled = true;
            };

            base.OnStartup(e);

            // * 常駐アイコンからのキャプチャ方法 ⇒ 通知アイコンの専用イベントを適せん追加？
            //   常駐アイコン？常駐サービス？
            //
            // * ウィンドウ管理 ⇒ WindowService
            //     ⇒ GetMainWindow
            //     ⇒ GetOptionWindow
            //
            // * ホットキーの管理 ⇒ HotkeyService
            //     ⇒ Initialize(); ※設定を元に登録（設定を監視し、ホットキー変更時に再設定する）
            //     ⇒ private Add(id, fsModifiers, vk, x => CaptureService.CaptureScreen());
            //     ⇒ private Clear();
            //
            // * キャプチャの管理 ⇒ CaptureService
            //     ⇒ Initialize(); ※各キャプチャの生成（設定を監視し、各キャプチャへ反映する）
            //     ⇒ CaptureScreen
            //     ⇒ CaptureWindow
            //     ⇒ CapturePage
            //  状態管理：待機中or処理中
        }

        /// <summary>
        /// 終了イベント
        /// </summary>
        /// <param name="e">イベント引数</param>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            this.Dispose();
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

        #region IDisposable members
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
