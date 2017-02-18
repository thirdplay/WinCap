using System;
using WinCap.Models;
using WinCap.Serialization;
using WinCap.ViewModels;
using WinCap.Views;

namespace WinCap
{
    /// <summary>
    /// アプリケーションのアクション機能を提供します。
    /// </summary>
    public class ApplicationAction
    {
        /// <summary>
        /// アプリケーションのインスタンス
        /// </summary>
        private readonly Application application;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="application">アプリケーションのインスタンス</param>
        public ApplicationAction(Application application)
        {
            this.application = application;
        }

        /// <summary>
        /// ショートカットを作成します。
        /// </summary>
        public void CreateShortcut()
        {
            var shortcut = new StartupShortcut();
            var desktopShortcut = new DesktopShortcut();

            shortcut.Recreate(Settings.General.IsRegisterInStartup);
            desktopShortcut.Recreate(Settings.General.IsCreateShortcutToDesktop);
        }

        /// <summary>
        /// アクションを登録します。
        /// </summary>
        public void RegisterActions()
        {
            var settings = Settings.ShortcutKey;
            this.application.HookService.Register(() => settings.FullScreen.Value.ToShortcutKey(), () => this.application.CapturerService.CaptureDesktop());
            this.application.HookService.Register(() => settings.ActiveControl.Value.ToShortcutKey(), () => this.application.CapturerService.CaptureActiveControl());
            this.application.HookService.Register(() => settings.SelectionControl.Value.ToShortcutKey(), () => this.application.CapturerService.CaptureSelectionControl());
            this.application.HookService.Register(() => settings.WebPage.Value.ToShortcutKey(), () => this.application.CapturerService.CaptureWebPage());
        }

        /// <summary>
        /// 設定ウィンドウを表示します。
        /// </summary>
        public void ShowSettings()
        {
            using (this.application.HookService.Suspend())
            {
                if (SettingsWindow.Instance != null)
                {
                    SettingsWindow.Instance.Activate();
                }
                else
                {
                    SettingsWindow.Instance = new SettingsWindow()
                    {
                        DataContext = new SettingsWindowViewModel(this.application.HookService, this)
                    };
                    SettingsWindow.Instance.ShowDialog();
                    SettingsWindow.Instance = null;
                }
            }
        }
    }
}
