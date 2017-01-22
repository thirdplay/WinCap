using System;
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
        /// 押下した修飾キー
        /// </summary>
        private ModifierKeys _pressedModifierKeys = ModifierKeys.None;

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
        public bool IsSuspended => this._suspended;

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
            this._pressedModifierKeys = ModifierKeys.None;
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
                this._pressedModifierKeys |= args.KeyCode.GetModifierKeys();
            }
            else
            {
                var pressedEventArgs = new ShortcutKeyPressedEventArgs(args.KeyCode.ToKey(), this._pressedModifierKeys);
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

            if (args.KeyCode.IsModifyKey())
            {
                this._pressedModifierKeys ^= args.KeyCode.GetModifierKeys();
            }
        }
    }
}
