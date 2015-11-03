using System;
using System.Collections.Generic;
using System.Windows;
using WinCap.Views;
using System.Reactive.Linq;
using WinCap.ViewModels;
using WinCap.Utilities.Lifetime;
using Livet;

namespace WinCap.Services
{
    /// <summary>
    /// ウィンドウサービス
    /// </summary>
    public sealed class WindowService : IDisposableHolder
    {
        #region フィールド
        /// <summary>
        /// 基本CompositeDisposable
        /// </summary>
        private readonly LivetCompositeDisposable compositeDisposable = new LivetCompositeDisposable();

        /// <summary>
        /// ウィンドウコンテナ
        /// </summary>
        private Dictionary<string, Window> container = new Dictionary<string, Window>();

        /// <summary>
        /// 選択ウィンドウVM
        /// </summary>
        private SelectWindowViewModel selectWindowVm;
        #endregion

        #region プロパティ
        /// <summary>
        /// 現在のウィンドウサービス
        /// </summary>
        public static WindowService Current { get; } = new WindowService();
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private WindowService() { }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            selectWindowVm = new SelectWindowViewModel();
        }

        /// <summary>
        /// メインウィンドウ取得
        /// </summary>
        /// <returns>メインウィンドウ</returns>
        public MainWindow GetMainWindow()
        {
            string key = nameof(MainWindow);
            if (!container.ContainsKey(key))
            {
                this.container.Add(key, new MainWindow());
                Observable.FromEventPattern(
                    handler => this.container[key].Closed += handler,
                    handler => this.container[key].Closed -= handler
                )
                .Subscribe(x => this.container.Remove(x.Sender.GetType().Name));
            }
            return container[key] as MainWindow;
        }

        /// <summary>
        /// 選択ウィンドウ取得
        /// </summary>
        /// <returns>選択ウィンドウ</returns>
        public SelectWindow GetSelectWindow()
        {
            string key = nameof(SelectWindow);
            if (!container.ContainsKey(key))
            {
                this.selectWindowVm.Initialize();
                this.container.Add(key, new SelectWindow() { DataContext = this.selectWindowVm });
                Observable.FromEventPattern(
                    handler => this.container[key].Closed += handler,
                    handler => this.container[key].Closed -= handler
                )
                .Subscribe(x => this.container.Remove(x.Sender.GetType().Name));
            }
            return container[key] as SelectWindow;
        }

        /// <summary>
        /// ウィンドウ生成
        /// </summary>
        /// <remarks>
        /// ウィンドウを生成し、クローズイベントを監視して後片付けをする。
        /// </remarks>
        /// <param name="key">ウィンドウキー</param>
        /// <param name="dataContext">データコンテキスト</param>
        /// <typeparam name="T">ウィンドウを継承したクラス</typeparam>
        //private void createWindow<T>(string key, object dataContext = null) where T : Window, new()
        //{
        //    container.Add(key, new T() { DataContext = dataContext });
        //    Observable.FromEventPattern(
        //        handler => this.container[key].Closed += handler,
        //        handler => this.container[key].Closed -= handler
        //    )
        //    .Subscribe(x => this.container.Remove(x.Sender.GetType().Name));
        //}

        #region IDisposableHoloder members
        ICollection<IDisposable> IDisposableHolder.CompositeDisposable => this.compositeDisposable;

        /// <summary>
        /// このインスタンスによって使用されているリソースを全て破棄する。
        /// </summary>
        public void Dispose()
        {
            foreach (Window window in container.Values)
            {
                window.Close();
            }
            this.compositeDisposable.Dispose();
        }
        #endregion
    }
}
