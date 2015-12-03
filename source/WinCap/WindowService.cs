﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Reactive.Linq;
using WinCap.Views;
using WinCap.ViewModels;
using Livet;
using MetroTrilithon.Lifetime;

namespace WinCap
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
        private WindowService() { }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            controlSelectWindow = new ControlSelectionWindowViewModel().AddTo(this);
            settingWindow = new SettingWindowViewModel().AddTo(this);
        }

        /// <summary>
        /// メインウィンドウを取得します。
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
        /// コントロール選択ウィンドウを取得します。
        /// </summary>
        /// <returns>選択ウィンドウ</returns>
        public SettingWindow GetSettingWindow()
        {
            string key = nameof(SettingWindow);
            if (!container.ContainsKey(key))
            {
                createWindow<SettingWindow>(key, this.settingWindow);
            }
            return container[key] as SettingWindow;
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