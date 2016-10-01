using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;

namespace WinCap.Services
{
    /// <summary>
    /// ショートカットキーの検出機能を提供します。
    /// </summary>
    public class HookService : IDisposable
    {
        private readonly ShortcutKeyDetector _detector = new ShortcutKeyDetector();
        private readonly List<HookAction> _hookActions = new List<HookAction>();
        private int _suspendRequestCount;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HookService()
        {
            this._detector.Pressed += this.KeyHookOnPressed;
            this._detector.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDisposable Suspend()
        {
            this._suspendRequestCount++;
            this._detector.Stop();

            return Disposable.Create(() =>
            {
                this._suspendRequestCount--;
                if (this._suspendRequestCount == 0)
                {
                    this._detector.Start();
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shortcutKey"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public IDisposable Register(ShortcutKey shortcutKey, Action action)
        {
            return this.Register(shortcutKey, action, () => true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shortcutKey"></param>
        /// <param name="action"></param>
        /// <param name="canExecute"></param>
        /// <returns></returns>
        public IDisposable Register(ShortcutKey shortcutKey, Action action, Func<bool> canExecute)
        {
            var hook = new HookAction(shortcutKey, action, canExecute);
            this._hookActions.Add(hook);

            return Disposable.Create(() => this._hookActions.Remove(hook));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void KeyHookOnPressed(object sender, ShortcutKeyPressedEventArgs args)
        {
            if (args.ShortcutKey == ShortcutKey.None) { return; }

            var target = this._hookActions.FirstOrDefault(x => x.ShortcutKey == args.ShortcutKey);
            if (target != null && target.CanExecute())
            {
                Application.Current.Dispatcher.Invoke(() => target.Action());
                args.Handled = true;
            }
        }

        /// <summary>
        /// このインスタンスによって使用されているリソースを全て破棄します。
        /// </summary>
        public void Dispose()
        {
            this._detector.Stop();
        }

        /// <summary>
        /// キーフックアクションを表すクラスです。
        /// </summary>
        private class HookAction
        {
            /// <summary>
            /// ショートカットキー
            /// </summary>
            public ShortcutKey ShortcutKey { get; }

            /// <summary>
            /// アクション
            /// </summary>
            public Action Action { get; }

            /// <summary>
            /// 実行可否判定関数
            /// </summary>
            public Func<bool> CanExecute { get; }

            public HookAction(ShortcutKey shortcutKey, Action action, Func<bool> canExecute)
            {
                this.ShortcutKey = shortcutKey;
                this.Action = action;
                this.CanExecute = canExecute;
            }
        }
    }
}
