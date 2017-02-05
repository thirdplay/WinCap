using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WinCap.Models;
using WinCap.Serialization;

namespace WinCap.Views.Controls
{
    /// <summary>
    /// ショートカットキーの読み取り、表示を行うテキストボックス。
    /// </summary>
    public class ShortcutKeyBox : TextBox
    {
        /// <summary>
        /// ショートカットキー検出器
        /// </summary>
        private readonly ShortcutKeyDetector detector = new ShortcutKeyDetector();

        /// <summary>
        /// 静的なコンストラクタ。
        /// </summary>
        static ShortcutKeyBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ShortcutKeyBox),
                new FrameworkPropertyMetadata(typeof(ShortcutKeyBox)));
        }

        /// <summary>
        /// 押下した修飾キーセット
        /// </summary>
        private ModifierKeys pressedModifiers = ModifierKeys.None;

        /// <summary>
        /// 押下したキー
        /// </summary>
        private Key pressedKey = Key.None;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public ShortcutKeyBox()
        {
            this.detector.Pressed += this.KeyHookOnPressed;
        }

        #region Current 依存関係プロパティ

        /// <summary>
        /// 現在のショートカットキーのキーコードを取得します。
        /// </summary>
        public int[] Current
        {
            get { return (int[])this.GetValue(CurrentProperty); }
            set { this.SetValue(CurrentProperty, value); }
        }

        public static readonly DependencyProperty CurrentProperty =
            DependencyProperty.Register(nameof(Current), typeof(int[]), typeof(ShortcutKeyBox), new UIPropertyMetadata(null, CurrentPropertyChangedCallback));

        private static void CurrentPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var instance = (ShortcutKeyBox)d;
            instance.UpdateText();
        }

        /// <summary>
        /// 現在のショートカットキーを取得します。
        /// </summary>
        private ShortcutKey? CurrentAsKeys
        {
            get { return this.Current?.ToShortcutKey(); }
            set { this.Current = value?.ToSerializable(); }
        }
        #endregion

        /// <summary>
        /// キーボードフォーカスを受け取ったときに発生するイベント。
        /// </summary>
        /// <param name="e">イベント引数</param>
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);

            this.detector.Start();
            this.UpdateText();
        }

        /// <summary>
        /// キーボードフォーカスを失ったときに発生するイベント。
        /// </summary>
        /// <param name="e">イベント引数</param>
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);

            this.detector.Stop();
        }

        /// <summary>
        /// キーボードのキーを押下したときに発生するイベント。
        /// </summary>
        /// <param name="e">イベント引数</param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (!e.IsRepeat)
            {
                var key = e.Key == Key.System ? e.SystemKey : e.Key;
                ExecuteKeyDown(key);
            }

            e.Handled = true;
            base.OnPreviewKeyDown(e);
        }

        /// <summary>
        /// キーボードのキーを離したときに発生するイベント。
        /// </summary>
        /// <param name="e">イベント引数</param>
        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            if (!e.IsRepeat)
            {
                var key = e.Key == Key.System ? e.SystemKey : e.Key;
                if (key.IsModifyKey())
                {
                    this.pressedModifiers ^= key.GetModifierKeys();
                }

                this.pressedKey = Key.None;
                this.UpdateText();
            }

            e.Handled = true;
            base.OnPreviewKeyUp(e);
        }

        /// <summary>
        /// ショートカットキー検出時に発生するイベント。
        /// </summary>
        /// <param name="sender">イベント発生元オブジェクト</param>
        /// <param name="args">イベント引数</param>
        private void KeyHookOnPressed(object sender, ShortcutKeyPressedEventArgs args)
        {
            if (args.ShortcutKey == ShortcutKey.None) { return; }

            if (args.ShortcutKey.Key == Key.PrintScreen)
            {
                ExecuteKeyDown(Key.PrintScreen);
            }
        }

        /// <summary>
        /// キー押下時の処理を実行します。
        /// </summary>
        /// <param name="key">キーコード</param>
        private void ExecuteKeyDown(Key key)
        {
            if (key == Key.Back)
            {
                this.pressedModifiers = ModifierKeys.None;
                this.pressedKey = Key.None;
            }
            else if (key.IsModifyKey())
            {
                this.pressedModifiers |= key.GetModifierKeys();
            }
            else
            {
                this.pressedKey = key;
            }

            this.CurrentAsKeys = this.pressedKey != Key.None
                ? this.GetShortcutKey()
                : ShortcutKey.None;

            this.UpdateText();
        }

        /// <summary>
        /// テキストを更新します。
        /// </summary>
        private void UpdateText()
        {
            var text = (this.CurrentAsKeys ?? this.GetShortcutKey()).ToString();

            this.Text = text;
            this.CaretIndex = text.Length;
        }

        /// <summary>
        /// ショートカットキーを取得します。
        /// </summary>
        /// <returns>ショートカットキー</returns>
        private ShortcutKey GetShortcutKey()
        {
            return new ShortcutKey(this.pressedKey, this.pressedModifiers);
        }
    }
}
