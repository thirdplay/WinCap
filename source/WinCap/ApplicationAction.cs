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
        private readonly Application _application;

        /// <summary>
        /// 基本CompositeDisposable
        /// </summary>
        private readonly LivetCompositeDisposable _compositeDisposable = new LivetCompositeDisposable();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="application">アプリケーションのインスタンス</param>
        public ApplicationAction(Application application)
        {
            this._application = application;
            this.RegisterActions();
        }

        /// <summary>
        /// アクションを登録します。
        /// </summary>
        public void RegisterActions()
        {
            var settings = Settings.ShortcutKey;

            this._compositeDisposable.Add(this._application.HookService
                .Register(settings.FullScreen.Value.ToShortcutKey(), () => this._application.CapturerService.CaptureDesktop()));

            this._compositeDisposable.Add(this._application.HookService
                .Register(settings.ActiveControl.Value.ToShortcutKey(), () => this._application.CapturerService.CaptureActiveControl()));

            this._compositeDisposable.Add(this._application.HookService
                .Register(settings.SelectionControl.Value.ToShortcutKey(), () => this._application.CapturerService.CaptureSelectionControl()));

            this._compositeDisposable.Add(this._application.HookService
                .Register(settings.WebPage.Value.ToShortcutKey(), () => this._application.CapturerService.CaptureWebPage()));
        }

        /// <summary>
        /// アクションの登録を解除します。
        /// </summary>
        public void DeregisterActions()
        {
            foreach (var register in this._compositeDisposable)
            {
                register.Dispose();
            }
            this._compositeDisposable.Clear();
        }

        /// <summary>
        /// 設定ウィンドウを表示します。
        /// </summary>
        /// <returns>設定ウィンドウ</returns>
        public SettingsWindow ShowSettings()
        {
            var window = this._application.WindowService.GetSettingsWindow(x =>
            {
                if (x.DialogResult)
                {
                    LocalSettingsProvider.Instance.Save();
                    this._application.CreateShortcut();
                }
            });
            window.Show();
            window.Activate();

            return window;
        }

        #region IDisposableHoloder members
        ICollection<IDisposable> IDisposableHolder.CompositeDisposable => this._compositeDisposable;

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
