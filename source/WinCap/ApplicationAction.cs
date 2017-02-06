using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using WinCap.Models;
using WinCap.Serialization;
using WinCap.Views;
using WinCap.Properties;
using WinCap.ViewModels;

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
        /// 登録に失敗したショートカットキー登録情報
        /// </summary>
        public readonly List<ShortcutKeyRegisterInfo> FailedRegisters = new List<ShortcutKeyRegisterInfo>();

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

            shortcut.Recreate(Serialization.Settings.General.IsRegisterInStartup);
            desktopShortcut.Recreate(Serialization.Settings.General.IsCreateShortcutToDesktop);
        }

        /// <summary>
        /// アクションを登録します。
        /// </summary>
        public void RegisterActions()
        {
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
            this.FailedRegisters.Clear();
            foreach (var registerInfo in shortcutkeies)
            {
                if (!hookService.Register(registerInfo.ShortcutKey, registerInfo.Action))
                {
                    this.FailedRegisters.Add(registerInfo);
                }
            }

            // 登録失敗の場合、設定を変更するか確認し、設定ウィンドウを表示する
            if (this.FailedRegisters.Count > 0)
            {
                var sb = new StringBuilder();
                foreach (var registerInfo in this.FailedRegisters)
                {
                    sb.Append($"{registerInfo.Name} ({registerInfo.ShortcutKey.ToString()})\n");
                }
                var result = MessageBox.Show(string.Format(Resources.Settings_ShortcutKeyUnusable, sb), ProductInfo.Title, MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    this.ShowSettings(true);
                }
            }
        }

        /// <summary>
        /// アクションの登録を解除します。
        /// </summary>
        public void DeregisterActions()
        {
            this.application.HookService.Unregister();
            this.FailedRegisters.Clear();
        }

        /// <summary>
        /// 設定ウィンドウを表示します。
        /// </summary>
        /// <param name="isShortcutKey">ショートカットキー設定を初期表示するかどうか</param>
        /// <returns>設定ウィンドウ</returns>
        public SettingsWindow ShowSettings(bool isShortcutKey = false)
        {
            var window = this.application.WindowService.GetSettingsWindow(
                () =>
                {
                    var viewMode = new SettingsWindowViewModel();
                    if (isShortcutKey)
                    {
                        viewMode.SelectedItem = viewMode.ShortcutKey;
                    }
                    return viewMode;
                },
                (x) =>
                {
                    if (x.DialogResult)
                    {
                        LocalSettingsProvider.Instance.Save();
                        this.CreateShortcut();
                        this.RegisterActions();
                    }
                }
            );
            window.Show();
            window.Activate();

            return window;
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
