using Livet.Behaviors.Messaging;
using Livet.Messaging;
using System.Windows;
using WinCap.ViewModels.Messages;

namespace WinCap.Views.Behaviors
{
    /// <summary>
    /// ダイアログ結果を設定するアクション
    /// </summary>
    public class SetDialogResultAction : InteractionMessageAction<Window>
    {
        /// <summary>
        /// アクション実行
        /// </summary>
        /// <param name="message">メッセージ</param>
        protected override void InvokeAction(InteractionMessage message)
        {
            var setDialogResultMessage = message as SetDialogResultMessage;
            if (setDialogResultMessage == null) { return; }

            this.AssociatedObject.DialogResult = setDialogResultMessage.DialogResult;
        }
    }
}
