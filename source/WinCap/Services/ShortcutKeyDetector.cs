using System;
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
        private readonly HashSet<Keys> pressedModifiers = new HashSet<Keys>();

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
            this.pressedModifiers.Clear();
        }

        /// <summary>
        /// キーダウン割り込みイベント
        /// </summary>
        /// <param name="sender">イベント発生元オブジェクト</param>
        /// <param name="args">イベント引数</param>
        private void interceptorOnKeyDown(object sender, KeyEventArgs args)
        {
            if (this.suspended) { return; }

            if (args.KeyCode.IsModifyKey())
            {
                this.pressedModifiers.Add(args.KeyCode);
            }
            else
            {
                var pressedEventArgs = new ShortcutKeyPressedEventArgs(args.KeyCode, this.pressedModifiers);
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
            if (this.pressedModifiers.Count == 0) { return; }

            if (args.KeyCode.IsModifyKey())
            {
                this.pressedModifiers.Remove(args.KeyCode);
            }
        }
    }
}
