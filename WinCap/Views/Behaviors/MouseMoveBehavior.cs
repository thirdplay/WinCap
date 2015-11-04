using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace WinCap.Views.Behaviors
{
    /// <summary>
    /// マウス移動ビヘイビア
    /// </summary>
    public class MouseMoveBehavior : Behavior<UIElement>
    {
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(MouseMoveBehavior), new UIPropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.MouseMove += OnMouseMove;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.MouseMove -= OnMouseMove;
        }

        /// <summary>
        /// マウス移動イベント
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (Command == null) return;
            if (!Command.CanExecute(e)) return;
            Command.Execute(e);
        }
    }
}
