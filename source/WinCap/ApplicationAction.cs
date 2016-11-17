using Livet;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using WinCap.Serialization;
using WinCap.Util.Lifetime;
using WinCap.ViewModels;
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
                .Register(settings.FullScreen, () => this.application.CapturerService.CaptureDesktop()));

            this.compositeDisposable.Add(this.application.HookService
                .Register(settings.ActiveControl, () => this.application.CapturerService.CaptureActiveControl()));

            this.compositeDisposable.Add(this.application.HookService
                .Register(settings.SelectionControl, () => this.application.CapturerService.CaptureSelectionControl()));

            this.compositeDisposable.Add(this.application.HookService
                .Register(settings.WebPage, () => this.application.CapturerService.CaptureWebPage()));
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
        public void ShowSettings()
        {
            if (this.application.SettingsWindow != null)
            {
                this.application.SettingsWindow.Activate();
                return;
            }

            var viewModel = new SettingsWindowViewModel(this.application.HookService, this);
            this.application.SettingsWindow = new SettingsWindow { DataContext = viewModel };
            Observable.FromEventPattern<EventArgs>(this.application.SettingsWindow, nameof(this.application.SettingsWindow.Closed))
            .Subscribe(x =>
            {
                if (viewModel.DialogResult)
                {
                    LocalSettingsProvider.Instance.Save();
                    this.application.CreateShortcut();
                }
                this.application.SettingsWindow = null;
            });
            this.application.SettingsWindow.Show();
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
