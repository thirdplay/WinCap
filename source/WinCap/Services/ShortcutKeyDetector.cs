﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Open.WinKeyboardHook;
using ModifierKeys = System.Windows.Input.ModifierKeys;

namespace WinCap.Services
{
    /// <summary>
    /// ショートカットキーを検出する機能を提供します。
    /// ([modifier key(s)] + [key] style)
    /// </summary>
    public class ShortcutKeyDetector
    {
        /// <summary>
        /// 押下した装飾キー
        /// </summary>
        private ModifierKeys pressedModifierKeys = ModifierKeys.None;

        /// <summary>
        /// キーボード監視
        /// </summary>
        private readonly IKeyboardInterceptor interceptor = new KeyboardInterceptor();

        /// <summary>
        /// 開始状態
        /// </summary>
        private bool started;

        /// <summary>
        /// 停止状態
        /// </summary>
        private bool suspended;

        /// <summary>
        /// 停止状態
        /// </summary>
        public bool IsSuspended { get { return suspended; } }

        /// <summary>
        /// ショートカットキー検出イベント
        /// </summary>
        public event EventHandler<ShortcutKeyPressedEventArgs> Pressed;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ShortcutKeyDetector()
        {
            this.interceptor.KeyDown += this.interceptorOnKeyDown;
            this.interceptor.KeyUp += this.interceptorOnKeyUp;
        }

        /// <summary>
        /// ショートカットの監視を開始します。
        /// </summary>
        public void Start()
        {
            if (!this.started)
            {
                this.interceptor.StartCapturing();
                this.started = true;
            }

            this.suspended = false;
        }

        /// <summary>
        /// ショートカットの監視を停止します。
        /// </summary>
        public void Stop()
        {
            this.suspended = true;
            this.pressedModifierKeys = ModifierKeys.None;
        }

        /// <summary>
        /// キーダウン割り込みイベント
        /// </summary>
        /// <param name="sender">イベント発生元オブジェクト</param>
        /// <param name="args">イベント引数</param>
        private void interceptorOnKeyDown(object sender, KeyEventArgs args)
        {
            if (this.suspended) { return; }

            if (args.KeyData.IsModifyKey())
            {
                this.pressedModifierKeys = args.KeyData.GetModifierKeys();
            }
            else
            {
                var pressedEventArgs = new ShortcutKeyPressedEventArgs(args.KeyCode.ToKey(), this.pressedModifierKeys);
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
        private void interceptorOnKeyUp(object sender, KeyEventArgs args)
        {
            if (this.suspended) { return; }

            if (args.KeyCode.IsModifyKey())
            {
                this.pressedModifierKeys = args.KeyData.GetModifierKeys();
            }
        }
    }
}
