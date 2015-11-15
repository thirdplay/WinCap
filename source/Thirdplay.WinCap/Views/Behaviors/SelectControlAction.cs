using Livet.Behaviors.Messaging;
using Livet.Messaging;
using WinCap.ViewModels.Messages;

namespace WinCap.Views.Behaviors
{
    /// <summary>
    /// コントロール選択ウィンドウのメソッドを呼び出すアクション
    /// </summary>
    public class SelectControlAction : InteractionMessageAction<ControlSelectWindow>
    {
        /// <summary>
        /// アクション実行
        /// </summary>
        /// <param name="message">メッセージ</param>
        protected override void InvokeAction(InteractionMessage message)
        {
            var controlSelectMessage = message as SelectControlMessage;
            if (controlSelectMessage == null) return;

            this.AssociatedObject.Select(controlSelectMessage.Handle);
        }
    }
}
