//using Livet;
//using System;
//using System.Collections.Generic;
//using System.Reactive.Linq;
//using System.Windows;
//using WinCap.Util.Lifetime;
//using WinCap.ViewModels;
//using WinCap.Views;

//namespace WinCap.Services
//{
//    /// <summary>
//    /// 各ウィンドウを取得する機能を提供します。
//    /// </summary>
//    public sealed class WindowService : IDisposableHolder
//    {
//        #region フィールド
//        /// <summary>
//        /// 基本CompositeDisposable
//        /// </summary>
//        private readonly LivetCompositeDisposable compositeDisposable = new LivetCompositeDisposable();

//        /// <summary>
//        /// ウィンドウコンテナ
//        /// </summary>
//        private Dictionary<string, Window> container = new Dictionary<string, Window>();

//        /// <summary>
//        /// ViewModelコンテナ
//        /// </summary>
//        private Dictionary<string, ViewModel> viewModels = new Dictionary<string, ViewModel>();
//        #endregion

//        #region プロパティ
//        /// <summary>
//        /// 現在のウィンドウサービス
//        /// </summary>
//        public static WindowService Current { get; } = new WindowService();
//        #endregion

//        /// <summary>
//        /// コンストラクタ
//        /// </summary>
//        private WindowService()
//        {
//            viewModels.Add(nameof(ControlSelectionWindow), new ControlSelectionWindowViewModel().AddTo(this));
//            viewModels.Add(nameof(SettingsWindow), new SettingsWindowViewModel().AddTo(this));
//        }

//        /// <summary>
//        /// コントロール選択ウィンドウを取得します。
//        /// </summary>
//        /// <returns>選択ウィンドウ</returns>
//        public ControlSelectionWindow GetControlSelectionWindow()
//        {
//            return CreateWindow<ControlSelectionWindow>(nameof(ControlSelectionWindow));
//        }

//        /// <summary>
//        /// 設定ウィンドウを取得します。
//        /// </summary>
//        /// <returns>設定ウィンドウ</returns>
//        public SettingsWindow GetSettingsWindow()
//        {
//            return CreateWindow<SettingsWindow>(nameof(SettingsWindow));
//        }

//        /// <summary>
//        /// ウィンドウを生成し、クローズイベントを監視して後片付けをする。
//        /// </summary>
//        /// <param name="key">ウィンドウキー</param>
//        /// <typeparam name="T">ウィンドウを継承したクラス</typeparam>
//        private T CreateWindow<T>(string key) where T : Window, new()
//        {
//            if (!container.ContainsKey(key))
//            {
//                container.Add(key, new T() { DataContext = viewModels[key] });
//                Observable.FromEventPattern(
//                    handler => this.container[key].Closed += handler,
//                    handler => this.container[key].Closed -= handler
//                )
//                .Subscribe(x => this.container.Remove(x.Sender.GetType().Name));
//            }
//            return container[key] as T;
//        }

//        #region IDisposableHoloder members
//        ICollection<IDisposable> IDisposableHolder.CompositeDisposable => this.compositeDisposable;

//        /// <summary>
//        /// このインスタンスによって使用されているリソースを全て破棄します。
//        /// </summary>
//        public void Dispose()
//        {
//            foreach (Window window in container.Values)
//            {
//                window.Close();
//            }
//            this.compositeDisposable.Dispose();
//        }
//        #endregion
//    }
//}
