using Livet;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows;
using WinCap.ViewModels;
using WinCap.Views;

namespace WinCap.Services
{
    /// <summary>
    /// ウィンドウの作成機能を提供します。
    /// </summary>
    public sealed class WindowService : IDisposable
    {
        /// <summary>
        /// ウィンドウコンテナ
        /// </summary>
        private readonly Dictionary<string, Window> container = new Dictionary<string, Window>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WindowService()
        {
        }

        /// <summary>
        /// ウィンドウを取得します。
        /// </summary>
        /// <param name="create">ViewModel生成メソッド</param>
        /// <param name="action">Closed時の処理メソッド</param>
        /// <returns>ウィンドウ</returns>
        public SettingsWindow GetSettingsWindow(Func<SettingsWindowViewModel> create, Action<SettingsWindowViewModel> action)
        {
            return GetWindow<SettingsWindow, SettingsWindowViewModel>(create, action);
        }

        /// <summary>
        /// ウィンドウが存在するかどうか確認します。
        /// </summary>
        public bool IsExists<T>() where T : Window, new()
        {
            return this.container.ContainsKey(typeof(T).Name);
        }

        /// <summary>
        /// ウィンドウを取得します。
        /// </summary>
        /// <typeparam name="T">ウィンドウクラスを継承したクラス</typeparam>
        /// <typeparam name="U">ViewModelを継承したクラス</typeparam>
        /// <param name="action">Closed時の処理メソッド</param>
        /// <returns>ウィンドウ</returns>
        private T GetWindow<T, U>(Func<U> create, Action<U> action)
            where T : Window, new()
            where U : ViewModel, new()
        {
            var key = typeof(T).Name;

            if (!this.container.ContainsKey(key))
            {
                var viewModel = create();
                var window = new T() { DataContext = viewModel };

                Observable.FromEventPattern<EventArgs>(window, nameof(window.Closed))
                .Subscribe(x =>
                {
                    this.container.Remove(key);
                    DispatcherHelper.UIDispatcher.Invoke(() => action(viewModel));
                });

                this.container.Add(key, window);
            }
            return this.container[key] as T;
        }

        #region IDisposable members
        /// <summary>
        /// このインスタンスによって使用されているリソースを全て破棄する。
        /// </summary>
        public void Dispose()
        {
            foreach (Window window in this.container.Values)
            {
                window.Close();
            }
            this.container.Clear();
        }
        #endregion
    }
}
