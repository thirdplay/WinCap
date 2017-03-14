using Livet;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows;
using WinCap.ViewModels;
using WinCap.Views;
using WpfUtility.Lifetime;
using WpfUtility.Mvvm;

namespace WinCap.Services
{
    /// <summary>
    /// ウィンドウを制御する機能を提供します。
    /// </summary>
    public sealed class WindowService : IDisposableHolder
    {
        /// <summary>
        /// 基本CompositeDisposable
        /// </summary>
        private readonly LivetCompositeDisposable compositeDisposable;

        /// <summary>
        /// ウィンドウを格納するコンテナ
        /// </summary>
        private readonly Dictionary<string, Window> container;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WindowService()
        {
            this.compositeDisposable = new LivetCompositeDisposable();
            this.container = new Dictionary<string, Window>();
        }

        /// <summary>
        /// コントロール選択ウィンドウを表示します。
        /// </summary>
        /// <returns>選択したウィンドウハンドル</returns>
        public IntPtr? ShowControlSelectionWindow()
        {
            var window = GetWindow<ControlSelectionWindow, ControlSelectionWindowViewModel>();
            var viewModel = window.DataContext as ControlSelectionWindowViewModel;
            window.ShowDialog();

            return viewModel.SelectedHandle;
        }

        /// <summary>
        /// ウィンドウを取得します。
        /// </summary>
        /// <typeparam name="TWindow">ウィンドウの型</typeparam>
        /// <typeparam name="TViewModel">ViewModelの型</typeparam>
        /// <returns>ウィンドウのインスタンス</returns>
        private TWindow GetWindow<TWindow, TViewModel>()
            where TWindow : Window, new()
            where TViewModel : WindowViewModel, new()
        {
            var windowName = typeof(TWindow).Name;
            if (!this.container.ContainsKey(windowName))
            {
                var viewModel = new TViewModel();
                var window = new TWindow()
                {
                    DataContext = viewModel
                };

                Observable.FromEventPattern(window, nameof(window.Closed))
                    .Subscribe(x => this.container.Remove(windowName))
                    .AddTo(this);

                Observable.FromEventPattern(window, nameof(window.Activated))
                    .Select(x => (x.Sender as TWindow).DataContext as TViewModel)
                    .Where(x => x.IsInitialized)
                    .Subscribe(_ => viewModel.Initialize())
                    .AddTo(this);

                this.container.Add(windowName, window);
            }

            return this.container[windowName] as TWindow;
        }

        #region IDisposableHoloder members

        ICollection<IDisposable> IDisposableHolder.CompositeDisposable => this.compositeDisposable;

        /// <summary>
        /// このインスタンスによって使用されているリソースを全て破棄します。
        /// </summary>
        public void Dispose()
        {
            this.compositeDisposable.Dispose();
        }

        #endregion
    }
}
