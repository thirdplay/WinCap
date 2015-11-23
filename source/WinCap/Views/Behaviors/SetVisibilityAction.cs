using Livet.Behaviors.Messaging;
using Livet.Messaging;
using System.Windows;
using WinCap.ViewModels.Messages;

namespace WinCap.Views.Behaviors
{
    /// <summary>
    /// 可視性を設定するアクション
    /// </summary>
    public class SetVisibilityAction : InteractionMessageAction<FrameworkElement>
    {
        /// <summary>
        /// アクション実行
        /// </summary>
        /// <param name="message">メッセージ</param>
        protected override void InvokeAction(InteractionMessage message)
        {
            var setVisibilityMessage = message as SetVisibilityMessage;
            if (setVisibilityMessage == null) return;

            this.AssociatedObject.Visibility = setVisibilityMessage.Visibility;
        }
    }
}
