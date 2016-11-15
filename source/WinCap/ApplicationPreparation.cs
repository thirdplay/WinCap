using System;
using WinCap.Interop;
using WinCap.Models;
using WinCap.Serialization;
using WinCap.Util.Lifetime;
using WinCap.ViewModels;
using WinCap.Views;
using PropResources = WinCap.Properties.Resources;

namespace WinCap
{
    /// <summary>
    /// アプリケーションの準備機能を提供します。
    /// </summary>
    public class ApplicationPreparation
    {
        /// <summary>
        /// アプリケーションのインスタンス
        /// </summary>
        private readonly Application application;

        /// <summary>
        /// 設定ウィンドウ
        /// </summary>
        private SettingsWindow settingsWindow;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="application">アプリケーションのインスタンス</param>
        public ApplicationPreparation(Application application)
        {
            this.application = application;
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
                new TaskTrayIconItem(PropResources.ContextMenu_Settings, () => this.showSettings()),
                new TaskTrayIconItem(PropResources.ContextMenu_Exit, () => { this.settingsWindow?.Close(); this.application.Shutdown(); }),
            };

            var taskTrayIcon = new TaskTrayIcon(icon, menus);
            taskTrayIcon.Show();
            taskTrayIcon.AddTo(this.application);
        }

        /// <summary>
        /// ショートカットを作成します。
        /// </summary>
        public void CreateShortcut()
        {
            var shortcut = new StartupShortcut();
            if (Settings.General.IsRegisterInStartup)
            {
                shortcut.Create();
            }
            else
            {
                shortcut.Remove();
            }

            var desktopShortcut = new DesktopShortcut();
            if (Settings.General.IsCreateShortcutToDesktop)
            {
                desktopShortcut.Create();
            }
            else
            {
                desktopShortcut.Remove();
            }
        }

        /// <summary>
        /// 設定ウィンドウを表示します。
        /// </summary>
        private void showSettings()
        {
            if (settingsWindow != null)
            {
                this.settingsWindow.Activate();
                return;
            }
            using (var viewModel = new SettingsWindowViewModel(this.application.HookService, this.application.ApplicationAction))
            {
                this.settingsWindow = new SettingsWindow
                {
                    DataContext = viewModel
                };
                var result = this.settingsWindow.ShowDialog();
                this.settingsWindow = null;
                if (result.HasValue && result.Value)
                {
                    LocalSettingsProvider.Instance.Save();
                    this.CreateShortcut();
                }
            }
        }
    }
}
