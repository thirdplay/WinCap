using System;
using System.Reactive.Disposables;
using System.Windows;
using WinCap.Models;

namespace WinCap.Services
{
    /// <summary>
    /// ショートカットキーの検出機能を提供します。
    /// </summary>
    public class HookService : IDisposable
    {
        /// <summary>
        /// ホットキー検出器
        /// </summary>
        private readonly HotkeyDetector detector;

        /// <summary>
        /// 停止リクエストカウンタ
        /// </summary>
        private int suspendRequestCount;

        /// <summary>
        /// ショートカット監視の停止状態
        /// </summary>
        public bool IsSuspended => this.detector.IsSuspended;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="window">メインウィンドウ</param>
        public HookService(Window window)
        {
            this.detector = new HotkeyDetector(window);
            this.detector.Start();
        }

        /// <summary>
        /// ショートカットの監視を停止します。
        /// </summary>
        /// <returns>停止リクエスト</returns>
        public IDisposable Suspend()
        {
            this.suspendRequestCount++;
            this.detector.Stop();

            return Disposable.Create(() =>
            {
                this.suspendRequestCount--;
                if (this.suspendRequestCount == 0)
                {
                    this.detector.Start();
                }
            });
        }

        /// <summary>
        /// ショートカットキーを登録します。
        /// </summary>
        /// <param name="shortcutKey">ショートカットキー</param>
        /// <param name="action">実行用メソッド</param>
        /// <returns>成功の場合はtrue、それ以外はfalseを返します。</returns>
        public bool Register(ShortcutKey shortcutKey, Action action)
        {
            return this.detector.Register(shortcutKey, action);
        }

        /// <summary>
        /// ショートカットキーをします。
        /// </summary>
        /// <param name="shortcutKey">ショートカットキー</param>
        /// <param name="action">実行用メソッド</param>
        public void Unregister()
        {
            this.detector.Unregister();
        }

        #region IDisposable members

        /// <summary>
        /// このインスタンスによって使用されているリソースを全て破棄します。
        /// </summary>
        public void Dispose()
        {
            this.detector.Dispose();
        }

        #endregion
    }
}
