using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace WinCap.Views.Behaviors
{
    /// <summary>
    /// EventCommandを呼び出すアクション
    /// </summary>
    public class CallEventAction : TriggerAction<DependencyObject>
    {
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(CallEventAction),
            new UIPropertyMetadata(null));

        /// <summary>
        /// EventCommand実行
        /// </summary>
        /// <param name="parameter">パラメータ</param>
        protected override void Invoke(object parameter)
        {
            if (parameter != null && this.AssociatedObject != null && this.Command != null)
            {
                if (this.Command.CanExecute(parameter))
                {
                    this.Command.Execute(parameter);
                }
            }
        }
    }
}
