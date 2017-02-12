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
#if true
            var settings = Settings.ShortcutKey;
            this.application.HookService.Register(() => settings.FullScreen.Value.ToShortcutKey(), () => this.application.CapturerService.CaptureDesktop());
            this.application.HookService.Register(() => settings.ActiveControl.Value.ToShortcutKey(), () => this.application.CapturerService.CaptureActiveControl());
            this.application.HookService.Register(() => settings.SelectionControl.Value.ToShortcutKey(), () => this.application.CapturerService.CaptureSelectionControl());
            this.application.HookService.Register(() => settings.WebPage.Value.ToShortcutKey(), () => this.application.CapturerService.CaptureWebPage());
#else
            var settings = Serialization.Settings.ShortcutKey;
            var captureService = this.application.CapturerService;
            var hookService = this.application.HookService;
            var shortcutkeies = new List<ShortcutKeyRegisterInfo>()
            {
                new ShortcutKeyRegisterInfo(Resources.Settings_DesktopCapture, settings.FullScreen.Value.ToShortcutKey(), () => captureService.CaptureDesktop()),
                new ShortcutKeyRegisterInfo(Resources.Settings_ActiveControlCapture, settings.ActiveControl.Value.ToShortcutKey(), () => captureService.CaptureActiveControl()),
                new ShortcutKeyRegisterInfo(Resources.Settings_SelectionControlCapture, settings.SelectionControl.Value.ToShortcutKey(), () => captureService.CaptureSelectionControl()),
                new ShortcutKeyRegisterInfo(Resources.Settings_WebPageCapture, settings.WebPage.Value.ToShortcutKey(), () => captureService.CaptureWebPage()),
            };

            // ショートカットキーを登録
            this.failedRegisters.Clear();
            foreach (var registerInfo in shortcutkeies)
            {
                if (!hookService.Register(registerInfo.ShortcutKey, registerInfo.Action))
                {
                    this.failedRegisters.Add(registerInfo);
                }
            }

            // 登録失敗の場合、設定を変更するか確認し、設定ウィンドウを表示する
            if (this.failedRegisters.Count > 0)
            {
                var sb = new StringBuilder();
                foreach (var registerInfo in this.failedRegisters)
                {
                    sb.Append($"{registerInfo.Name} ({registerInfo.ShortcutKey.ToString()})\n");
                }
                var result = MessageBox.Show(string.Format(Resources.Settings_ShortcutKeyUnusable, sb), ProductInfo.Title, MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    this.ShowSettings(TabIndexShortcutKey);
                }
            }
#endif
        }

        /// <summary>
        /// 設定ウィンドウを表示します。
        /// </summary>
        /// <returns>設定ウィンドウ</returns>
        public SettingsWindow ShowSettings()
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
            return null;
        }
    }
}
