using Livet;
using System;
using System.Reactive.Disposables;
using WinCap.Interop;
using WinCap.Serialization;
using WinCap.Util.Lifetime;
using WinCap.ViewModels;
using WinCap.Views;
using PropResources = WinCap.Properties.Resources;

namespace WinCap
{
    /// <summary>
    /// アプリケーションの準備を実施します。
    /// </summary>
    public class ApplicationPreparation
    {
        /// <summary>
        /// アプリケーションのインスタンス
        /// </summary>
        private readonly Application application;

        /// <summary>
        /// ショートカットキー登録リクエストのコンテナ
        /// </summary>
        private LivetCompositeDisposable registers = new LivetCompositeDisposable();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="application">アプリケーションのインスタンス</param>
        public ApplicationPreparation(Application application)
        {
            this.application = application;
            Disposable.Create(() => this.registers.Dispose());
        }

        /// <summary>
        /// アクションを登録します。
        /// </summary>
        public void RegisterActions()
        {
            var settings = Settings.ShortcutKey;

            this.registers.Add(this.application.HookService
                .Register(settings.FullScreen, () => this.application.CapturerService.CaptureDesktop()));

            this.registers.Add(this.application.HookService
                .Register(settings.ActiveControl, () => this.application.CapturerService.CaptureActiveControl()));

            this.registers.Add(this.application.HookService
                .Register(settings.SelectionControl, () => this.application.CapturerService.CaptureSelectionControl()));

            this.registers.Add(this.application.HookService
                .Register(settings.WebPage, () => this.application.CapturerService.CaptureWebPage()));
        }

        /// <summary>
        /// アクションの登録を解除します。
        /// </summary>
        public void DeregisterActions()
        {
            foreach (var register in this.registers)
            {
                register.Dispose();
            }
            this.registers.Clear();
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
                    new TaskTrayIconItem(PropResources.ContextMenu_DesktopCapture, () => this.application.CapturerService.CaptureDesktop()),
                    new TaskTrayIconItem(PropResources.ContextMenu_ControlCapture, () => this.application.CapturerService.CaptureSelectionControl()),
                    new TaskTrayIconItem(PropResources.ContextMenu_WebPageCapture, () => this.application.CapturerService.CaptureWebPage()),
                }),
                new TaskTrayIconItem(PropResources.ContextMenu_Settings, () => this.ShowSettings()),
                new TaskTrayIconItem(PropResources.ContextMenu_Exit, () => this.application.Shutdown()),
            };

            var taskTrayIcon = new TaskTrayIcon(icon, menus);
            taskTrayIcon.Show();
            taskTrayIcon.AddTo(this.application);
        }

        /// <summary>
        /// 設定ウィンドウを表示します。
        /// </summary>
        private void ShowSettings()
        {
            if (this.application.HookService.IsSuspended) { return; }
            using (this.application.HookService.Suspend())
            {
                this.DeregisterActions();

                var window = new SettingsWindow
                {
                    DataContext = new SettingsWindowViewModel()
                };
                window.ShowDialog();

                this.RegisterActions();
            }
        }
    }
}
