using Livet.Behaviors.Messaging;
using Livet.Messaging;
using System.Windows;
using WinCap.ViewModels.Messages;

namespace WinCap.Views.Behaviors
{
    /// <summary>
    /// ウィンドウの位置、サイズを設定するアクション
    /// </summary>
    public class SetWindowBoundsAction : InteractionMessageAction<Window>
    {
        /// <summary>
        /// アクション実行
        /// </summary>
        /// <param name="message">メッセージ</param>
        protected override void InvokeAction(InteractionMessage message)
        {
            var setWindowBoundsMessage = message as SetWindowBoundsMessage;
            if (setWindowBoundsMessage == null) return;

            if (setWindowBoundsMessage.Left.HasValue) this.AssociatedObject.Left = setWindowBoundsMessage.Left.Value;
            if (setWindowBoundsMessage.Top.HasValue) this.AssociatedObject.Top = setWindowBoundsMessage.Top.Value;
            if (setWindowBoundsMessage.Width.HasValue) this.AssociatedObject.Width = setWindowBoundsMessage.Width.Value;
            if (setWindowBoundsMessage.Height.HasValue) this.AssociatedObject.Height = setWindowBoundsMessage.Height.Value;
        }
    }
}
