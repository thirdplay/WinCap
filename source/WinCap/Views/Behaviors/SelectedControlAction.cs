using Livet.Behaviors.Messaging;
using Livet.Messaging;
using WinCap.ViewModels.Messages;

namespace WinCap.Views.Behaviors
{
    /// <summary>
    /// コントロール選択ウィンドウのメソッドを呼び出すアクション
    /// </summary>
    public class SelectedControlAction : InteractionMessageAction<ControlSelectionWindow>
    {
        /// <summary>
        /// アクション実行
        /// </summary>
        /// <param name="message">メッセージ</param>
        protected override void InvokeAction(InteractionMessage message)
        {
            var controlSelectMessage = message as SelectedControlMessage;
            if (controlSelectMessage == null) return;

            this.AssociatedObject.OnSelected(controlSelectMessage.Handle);
        }
    }
}
