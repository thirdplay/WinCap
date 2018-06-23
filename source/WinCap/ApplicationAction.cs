using System;
using System.Text;
using System.Windows;
using WinCap.Models;
using WinCap.Serialization;
using WinCap.ShortcutKeys;
using WinCap.ViewModels;
using WinCap.Views;
using ProductInfo = WinCap.Properties.ProductInfo;
using Resources = WinCap.Properties.Resources;

namespace WinCap
{
    /// <summary>
    /// アプリケーションのアクション機能を提供します。
    /// </summary>
    public class ApplicationAction : IDisposable
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
        /// <returns>true:登録成功orスキップ、false:登録失敗</returns>
        public bool RegisterActions()
        {
            var settings = Settings.ShortcutKey;
            var captureService = this.application.CapturerService;

            // ショートカットキーを登録
            var sb = new StringBuilder();
            sb.Append(RegisterAction(settings.FullScreen.Value.ToShortcutKey(), () => captureService.CaptureDesktop(), Resources.Settings_DesktopCapture));
            sb.Append(RegisterAction(settings.ActiveControl.Value.ToShortcutKey(), () => captureService.CaptureActiveControl(), Resources.Settings_ActiveControlCapture));
            sb.Append(RegisterAction(settings.SelectionControl.Value.ToShortcutKey(), () => captureService.CaptureSelectionControl(), Resources.Settings_SelectionControlCapture));
            sb.Append(RegisterAction(settings.SelectionRegion.Value.ToShortcutKey(), () => captureService.CaptureSelectionRegion(), Resources.Settings_SelectionRegionCapture));
            sb.Append(RegisterAction(settings.WebPage.Value.ToShortcutKey(), () => captureService.CaptureWebPage(), Resources.Settings_WebPageCapture));

            // 登録失敗したキーがある場合、再登録の確認をする
            if (sb.Length > 0)
            {
                var result = MessageBox.Show(
                    string.Format(Resources.Settings_ShortcutKeyUnusable, sb.ToString()), ProductInfo.Title, MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// アクションを登録します。
        /// </summary>
        /// <param name="shortcutKey">登録するショートカットキー</param>
        /// <param name="action">登録するアクション</param>
        /// <param name="name">登録するショートカット名</param>
        /// <returns>登録成功の場合は空文字、登録失敗の場合はエラー文字列</returns>
        private string RegisterAction(ShortcutKey shortcutKey, Action action, string name)
        {
            if (!this.application.HookService.Register(shortcutKey, action))
            {
                return $"{name} {shortcutKey.ToString()}\n";
            }
            return "";
        }

        /// <summary>
        /// アクションの登録を解除します。
        /// </summary>
        public void DeregisterActions()
        {
            this.application.HookService.Unregister();
        }

        /// <summary>
        /// 設定ウィンドウを表示します。
        /// </summary>
        public void ShowSettings()
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

        #region IDisposable members

        /// <summary>
        /// このインスタンスによって使用されているリソースを全て破棄します。
        /// </summary>
        public void Dispose()
        {
            this.DeregisterActions();
        }

        #endregion
    }
}