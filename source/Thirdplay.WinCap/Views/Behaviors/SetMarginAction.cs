using Livet.Behaviors.Messaging;
using Livet.Messaging;
using System.Windows;
using Thirdplay.WinCap.ViewModels.Messages;

namespace Thirdplay.WinCap.Views.Behaviors
{
    /// <summary>
    /// 余白を設定するアクション
    /// </summary>
    public class SetMarginAction : InteractionMessageAction<FrameworkElement>
    {
        protected override void InvokeAction(InteractionMessage message)
        {
            var setMessage = message as SetMarginMessage;
            if (setMessage == null) return;

            System.Console.WriteLine("SetMarginAction");
            var left = setMessage.Left ?? this.AssociatedObject.Margin.Left;
            var top = setMessage.Top ?? this.AssociatedObject.Margin.Top;
            var right = setMessage.Left ?? this.AssociatedObject.Margin.Right;
            var bottom = setMessage.Bottom ?? this.AssociatedObject.Margin.Bottom;
            this.AssociatedObject.Margin = new Thickness(left, top, right, bottom);
        }
    }
}
