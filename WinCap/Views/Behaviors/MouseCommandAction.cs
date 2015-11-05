using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace WinCap.Views.Behaviors
{
    /// <summary>
    /// マウス座標をCommandに渡すアクション
    /// </summary>
    public class MouseCommandAction : TriggerAction<DependencyObject>
    {
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(MouseCommandAction),
            new UIPropertyMetadata(null));

        /// <summary>
        /// コマンド実行
        /// </summary>
        /// <param name="parameter">パラメータ</param>
        protected override void Invoke(object parameter)
        {
            var element = this.AssociatedObject as IInputElement;
            var args = parameter as MouseEventArgs;
            if (args != null && element != null && this.Command != null)
            {
                var point = args.GetPosition(element);
                if (this.Command.CanExecute(point))
                {
                    this.Command.Execute(point);
                }
            }
        }
    }
}
