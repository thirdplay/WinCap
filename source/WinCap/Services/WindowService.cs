using Livet;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows;
using WinCap.Util.Lifetime;
using WinCap.ViewModels;
using WinCap.Views;

namespace WinCap.Services
{
    /// <summary>
    /// 各ウィンドウを取得する機能を提供します。
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
        /// コントロール選択ウィンドウViewModel
        /// </summary>
        private ControlSelectionWindowViewModel controlSelectWindow;

        /// <summary>
        /// 設定ウィンドウViewModel
        /// </summary>
        private SettingWindowViewModel settingWindow;
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
        private WindowService()
        {
            controlSelectWindow = new ControlSelectionWindowViewModel().AddTo(this);
            settingWindow = new SettingWindowViewModel().AddTo(this);
        }

        /// <summary>
        /// コントロール選択ウィンドウを取得します。
        /// </summary>
        /// <returns>選択ウィンドウ</returns>
        public ControlSelectionWindow GetControlSelectWindow()
        {
            string key = nameof(ControlSelectionWindow);
            if (!container.ContainsKey(key))
            {
                createWindow<ControlSelectionWindow>(key, this.controlSelectWindow);
            }
            return container[key] as ControlSelectionWindow;
        }

        /// <summary>
        /// 設定ウィンドウを取得します。
        /// </summary>
        /// <returns>設定ウィンドウ</returns>
        public SettingsWindow GetSettingsWindow()
        {
            string key = nameof(SettingsWindow);
            if (!container.ContainsKey(key))
            {
                createWindow<SettingsWindow>(key, this.settingWindow);
            }
            return container[key] as SettingsWindow;
        }

        /// <summary>
        /// ウィンドウを生成し、クローズイベントを監視して後片付けをする。
        /// </summary>
        /// <param name="key">ウィンドウキー</param>
        /// <param name="dataContext">データコンテキスト</param>
        /// <typeparam name="T">ウィンドウを継承したクラス</typeparam>
        private void createWindow<T>(string key, object dataContext = null) where T : Window, new()
        {
            container.Add(key, new T() { DataContext = dataContext });
            Observable.FromEventPattern(
                handler => this.container[key].Closed += handler,
                handler => this.container[key].Closed -= handler
            )
            .Subscribe(x => this.container.Remove(x.Sender.GetType().Name));
        }

        #region IDisposableHoloder members
        ICollection<IDisposable> IDisposableHolder.CompositeDisposable => this.compositeDisposable;

        /// <summary>
        /// このインスタンスによって使用されているリソースを全て破棄します。
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
