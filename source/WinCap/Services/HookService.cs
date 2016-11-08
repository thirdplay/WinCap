﻿using Livet;
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
        /// <summary>
        /// ショートカットキー検出器
        /// </summary>
        private readonly ShortcutKeyDetector detector = new ShortcutKeyDetector();

        /// <summary>
        /// フックアクションリスト
        /// </summary>
        private readonly List<HookAction> hookActions = new List<HookAction>();

        /// <summary>
        /// 停止リクエストカウンタ
        /// </summary>
        private int suspendRequestCount;

        /// <summary>
        /// ショートカット監視の停止状態
        /// </summary>
        public bool IsSuspended
        {
            get { return this.detector.IsSuspended; }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HookService()
        {
            this.detector.Pressed += this.keyHookOnPressed;
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
        /// <returns>登録リクエスト</returns>
        public IDisposable Register(ShortcutKey shortcutKey, Action action)
        {
            return this.Register(shortcutKey, action, () => true);
        }

        /// <summary>
        /// ショートカットキーを登録します。
        /// </summary>
        /// <param name="shortcutKey">ショートカットキー</param>
        /// <param name="action">実行用メソッド</param>
        /// <param name="canExecute">実行可否判定用メソッド</param>
        /// <returns>登録リクエスト</returns>
        public IDisposable Register(ShortcutKey shortcutKey, Action action, Func<bool> canExecute)
        {
            var hook = new HookAction(shortcutKey, action, canExecute);
            this.hookActions.Add(hook);

            return Disposable.Create(() => this.hookActions.Remove(hook));
        }

        /// <summary>
        /// ショートカットキー検出イベント
        /// </summary>
        /// <param name="sender">イベント発生元オブジェクト</param>
        /// <param name="args">イベント引数</param>
        private void keyHookOnPressed(object sender, ShortcutKeyPressedEventArgs args)
        {
            if (args.ShortcutKey == ShortcutKey.None) { return; }
            if (args.IsRepeat) { return; }

            var target = this.hookActions.FirstOrDefault(x => x.ShortcutKey == args.ShortcutKey);
            if (target != null && target.CanExecute())
            {
                DispatcherHelper.UIDispatcher.Invoke(() => target.Action());
                args.Handled = true;
            }
        }

        /// <summary>
        /// このインスタンスによって使用されているリソースを全て破棄します。
        /// </summary>
        public void Dispose()
        {
            this.detector.Stop();
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
            /// 実行用メソッド
            /// </summary>
            public Action Action { get; }

            /// <summary>
            /// 実行可否判定用メソッド
            /// </summary>
            public Func<bool> CanExecute { get; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="shortcutKey">ショートカットキー</param>
            /// <param name="action">実行用メソッド</param>
            /// <param name="canExecute">実行可否判定用メソッド</param>
            public HookAction(ShortcutKey shortcutKey, Action action, Func<bool> canExecute)
            {
                this.ShortcutKey = shortcutKey;
                this.Action = action;
                this.CanExecute = canExecute;
            }
        }
    }
}
