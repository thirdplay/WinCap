using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using WinCap.Views;
using System.Reactive.Linq;

namespace WinCap.Services
{
    /// <summary>
    /// ウィンドウサービス
    /// </summary>
    public sealed class WindowService : IDisposable
    {
        #region フィールド
        /// <summary>
        /// ウィンドウコンテナ
        /// </summary>
        private Dictionary<string, Window> container = new Dictionary<string, Window>();
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
                createWindow<MainWindow>(key);
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
                createWindow<SelectWindow>(key);
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
        /// <typeparam name="T">ウィンドウを継承したクラス</typeparam>
        private void createWindow<T>(string key) where T : Window, new()
        {
            container.Add(key, new T());
            Observable.FromEventPattern(
                handler => container[key].Closed += handler,
                handler => container[key].Closed -= handler
            )
            .Subscribe(x => container.Remove(x.Sender.GetType().Name));
        }

        #region IDisposable members
        /// <summary>
        /// このインスタンスによって使用されているリソースを全て破棄する。
        /// </summary>
        public void Dispose()
        {
            foreach (Window window in container.Values)
            {
                window.Close();
            }
        }
        #endregion
    }
}
