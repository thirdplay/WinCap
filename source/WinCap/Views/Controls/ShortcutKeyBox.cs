using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WinCap.Serialization;
using WinCap.Services;

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

        private readonly HashSet<Key> pressedModifiers = new HashSet<Key>();
        private Key pressedKey = Key.None;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public ShortcutKeyBox()
        {
            this.detector.Pressed += this.keyHookOnPressed;
        }

        #region Current 依存関係プロパティ

        /// <summary>
        /// 現在のショートカットキーのキーコード
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
            instance.updateText();
        }

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
            this.updateText();
        }

        /// <summary>
        /// キーボードフォーカスを失ったときに発生するイベント。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);

            this.detector.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (!e.IsRepeat)
            {
                var key = e.Key == Key.System ? e.SystemKey : e.Key;
                executeKeyDown(key);
            }

            e.Handled = true;
            base.OnPreviewKeyDown(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            if (!e.IsRepeat)
            {
                var key = e.Key == Key.System ? e.SystemKey : e.Key;
                if (key.IsModifyKey())
                {
                    this.pressedModifiers.Remove(key);
                }

                this.pressedKey = Key.None;
                this.updateText();
            }

            e.Handled = true;
            base.OnPreviewKeyUp(e);
        }

        /// <summary>
        /// ショートカットキー検出時に発生するイベント。
        /// </summary>
        /// <param name="sender">イベント発生元オブジェクト</param>
        /// <param name="args">イベント引数</param>
        private void keyHookOnPressed(object sender, ShortcutKeyPressedEventArgs args)
        {
            if (args.ShortcutKey == ShortcutKey.None) { return; }
            if (args.IsRepeat) { return; }

            if (args.ShortcutKey.Key == System.Windows.Forms.Keys.PrintScreen)
            {
                executeKeyDown(Key.PrintScreen);
            }
        }

        /// <summary>
        /// キー押下時の処理を実行します。
        /// </summary>
        /// <param name="key">キーコード</param>
        private void executeKeyDown(Key key)
        {
            if (key == Key.Back)
            {
                this.pressedModifiers.Clear();
                this.pressedKey = Key.None;
            }
            else if (key.IsModifyKey())
            {
                this.pressedModifiers.Add(key);
            }
            else
            {
                this.pressedKey = key;
            }

            this.CurrentAsKeys = this.pressedModifiers.Any() && this.pressedKey != Key.None
                ? this.getShortcutKey()
                : (ShortcutKey?)null;

            this.updateText();
        }

        /// <summary>
        /// テキストを更新します。
        /// </summary>
        private void updateText()
        {
            var text = (this.CurrentAsKeys ?? this.getShortcutKey()).ToString();

            this.Text = text;
            this.CaretIndex = text.Length;
        }

        /// <summary>
        /// ショートカットキーを取得します。
        /// </summary>
        /// <returns>ショートカットキー</returns>
        private ShortcutKey getShortcutKey()
        {
            return new ShortcutKey(
                this.pressedKey.ToVirtualKey(),
                this.pressedModifiers.Select(x => x.ToVirtualKey()).ToArray());
        }
    }
}
