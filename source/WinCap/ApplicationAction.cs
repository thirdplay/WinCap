using Livet;
using System;
using System.Collections.Generic;
using WinCap.Serialization;
using WinCap.Util.Lifetime;
using WinCap.Views;

namespace WinCap
{
    /// <summary>
    /// アプリケーションのアクション機能を提供します。
    /// </summary>
    public class ApplicationAction : IDisposableHolder
    {
        /// <summary>
        /// アプリケーションのインスタンス
        /// </summary>
        private readonly Application application;

        /// <summary>
        /// 基本CompositeDisposable
        /// </summary>
        private readonly LivetCompositeDisposable compositeDisposable = new LivetCompositeDisposable();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="application">アプリケーションのインスタンス</param>
        public ApplicationAction(Application application)
        {
            this.application = application;
            this.RegisterActions();
        }

        /// <summary>
        /// アクションを登録します。
        /// </summary>
        public void RegisterActions()
        {
            var settings = Settings.ShortcutKey;

            this.compositeDisposable.Add(this.application.HookService
                .Register(settings.FullScreen.Value.ToShortcutKey(), () => this.application.CapturerService.CaptureDesktop()));

            this.compositeDisposable.Add(this.application.HookService
                .Register(settings.ActiveControl.Value.ToShortcutKey(), () => this.application.CapturerService.CaptureActiveControl()));

            this.compositeDisposable.Add(this.application.HookService
                .Register(settings.SelectionControl.Value.ToShortcutKey(), () => this.application.CapturerService.CaptureSelectionControl()));

            this.compositeDisposable.Add(this.application.HookService
                .Register(settings.WebPage.Value.ToShortcutKey(), () => this.application.CapturerService.CaptureWebPage()));
        }

        /// <summary>
        /// アクションの登録を解除します。
        /// </summary>
        public void DeregisterActions()
        {
            foreach (var register in this.compositeDisposable)
            {
                register.Dispose();
            }
            this.compositeDisposable.Clear();
        }

        /// <summary>
        /// 設定ウィンドウを表示します。
        /// </summary>
        /// <returns>設定ウィンドウ</returns>
        public SettingsWindow ShowSettings()
        {
            var window = this.application.WindowService.GetSettingsWindow(x =>
            {
                if (x.DialogResult)
                {
                    LocalSettingsProvider.Instance.Save();
                    this.application.CreateShortcut();
                }
            });
            window.Show();
            window.Activate();

            return window;
        }

        #region IDisposableHoloder members
        ICollection<IDisposable> IDisposableHolder.CompositeDisposable => this.compositeDisposable;

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
