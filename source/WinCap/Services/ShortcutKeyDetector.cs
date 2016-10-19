﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Open.WinKeyboardHook;

namespace WinCap.Services
{
    /// <summary>
    /// ショートカットキーを検出する機能を提供します。
    /// ([modifier key(s)] + [key] style)
    /// </summary>
    public class ShortcutKeyDetector
    {
        /// <summary>
        /// 装飾キーリスト
        /// </summary>
        private readonly HashSet<Keys> _pressedModifiers = new HashSet<Keys>();

        /// <summary>
        /// キーボード監視
        /// </summary>
        private readonly IKeyboardInterceptor _interceptor = new KeyboardInterceptor();

        /// <summary>
        /// 開始状態
        /// </summary>
        private bool _started;

        /// <summary>
        /// 停止状態
        /// </summary>
        private bool _suspended;

        /// <summary>
        /// 停止状態
        /// </summary>
        public bool IsSuspended { get { return _suspended; } }

        /// <summary>
        /// ショートカットキー検出イベント
        /// </summary>
        public event EventHandler<ShortcutKeyPressedEventArgs> Pressed;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ShortcutKeyDetector()
        {
            this._interceptor.KeyDown += this.InterceptorOnKeyDown;
            this._interceptor.KeyUp += this.InterceptorOnKeyUp;
        }

        /// <summary>
        /// ショートカットの監視を開始します。
        /// </summary>
        public void Start()
        {
            if (!this._started)
            {
                this._interceptor.StartCapturing();
                this._started = true;
            }

            this._suspended = false;
        }

        /// <summary>
        /// ショートカットの監視を停止します。
        /// </summary>
        public void Stop()
        {
            this._suspended = true;
            this._pressedModifiers.Clear();
        }

        /// <summary>
        /// キーダウン割り込みイベント
        /// </summary>
        /// <param name="sender">イベント発生元オブジェクト</param>
        /// <param name="args">イベント引数</param>
        private void InterceptorOnKeyDown(object sender, KeyEventArgs args)
        {
            if (this._suspended) { return; }

            if (args.KeyCode.IsModifyKey())
            {
                this._pressedModifiers.Add(args.KeyCode);
            }
            else
            {
                var pressedEventArgs = new ShortcutKeyPressedEventArgs(args.KeyCode, this._pressedModifiers);
                this.Pressed?.Invoke(this, pressedEventArgs);
                if (pressedEventArgs.Handled)
                {
                    args.SuppressKeyPress = true;
                }
            }
        }

        /// <summary>
        /// キーアップ割り込みイベント
        /// </summary>
        /// <param name="sender">イベント発生元オブジェクト</param>
        /// <param name="args">イベント引数</param>
        private void InterceptorOnKeyUp(object sender, KeyEventArgs args)
        {
            if (this._suspended) { return; }
            if (this._pressedModifiers.Count == 0) { return; }

            if (args.KeyCode.IsModifyKey())
            {
                this._pressedModifiers.Remove(args.KeyCode);
            }
        }
    }
}
