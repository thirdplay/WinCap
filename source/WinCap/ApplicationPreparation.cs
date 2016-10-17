using System;
using WinCap.Interop;
using WinCap.Serialization;
using WinCap.Services;
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
        private readonly Application _application;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="application"></param>
        public ApplicationPreparation(Application application)
        {
            this._application = application;
        }

        /// <summary>
        /// アクションを登録します。
        /// </summary>
        public void RegisterActions()
        {
            var settings = Settings.ShortcutKey;

            this._application.HookService
                .Register(settings.FullScreen.ToShortcutKey(), () => _application.CapturerService.CaptureDesktop())
                .AddTo(this._application);

            this._application.HookService
                .Register(settings.ActiveControl.ToShortcutKey(), () => _application.CapturerService.CaptureActiveControl())
                .AddTo(this._application);

            this._application.HookService
                .Register(settings.SelectionControl.ToShortcutKey(), () => _application.CapturerService.CaptureSelectionControl())
                .AddTo(this._application);

            this._application.HookService
                .Register(settings.WebPage.ToShortcutKey(), () => _application.CapturerService.CaptureWebPage())
                .AddTo(this._application);
        }

        /// <summary>
        /// タスクトレイアイコンを表示します。
        /// </summary>
        public void ShowTaskTrayIcon()
        {
            const string iconUri = "pack://application:,,,/WinCap;component/Assets/app.ico";
            Uri uri;
            if (!Uri.TryCreate(iconUri, UriKind.Absolute, out uri)) return;

            var icon = IconHelper.GetIconFromResource(uri);
            var menus = new[]
            {
                new TaskTrayIconItem(PropResources.ContextMenu_Capture, new[]
                {
                    new TaskTrayIconItem(PropResources.ContextMenu_DesktopCapture, () => this._application.CapturerService.CaptureDesktop()),
                    new TaskTrayIconItem(PropResources.ContextMenu_ControlCapture, () => this._application.CapturerService.CaptureSelectionControl()),
                    new TaskTrayIconItem(PropResources.ContextMenu_WebPageCapture, () => this._application.CapturerService.CaptureWebPage()),
                }),
                new TaskTrayIconItem(PropResources.ContextMenu_Settings, () => this.ShowSettings()),
                new TaskTrayIconItem(PropResources.ContextMenu_Exit, () => this._application.Shutdown()),
            };

            var taskTrayIcon = new TaskTrayIcon(icon, menus);
            taskTrayIcon.Show();
            taskTrayIcon.AddTo(this._application);
        }

        /// <summary>
        /// 設定ウィンドウを表示します。
        /// </summary>
        private void ShowSettings()
        {
            //if (this._application.SettingsWindow != null)
            //{
            //    this._application.SettingsWindow.Activate();
            //    return;
            //}
            //using (this._application.HookService.Suspend())
            //{
            //    this._application.SettingsWindow = new SettingsWindow
            //    {
            //        DataContext = new SettingsWindowViewModel()
            //    };
            //    this._application.SettingsWindow.ShowDialog();
            //}
        }
    }
}
