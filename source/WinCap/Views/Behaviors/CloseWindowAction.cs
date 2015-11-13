using Livet.Behaviors.Messaging;
using Livet.Messaging;
using System.Windows;
using WinCap.ViewModels.Messages;

namespace WinCap.Views.Behaviors
{
    /// <summary>
    /// ウィンドウを閉じるアクション
    /// </summary>
    public class CloseWindowAction : InteractionMessageAction<Window>
    {
        /// <summary>
        /// アクション実行
        /// </summary>
        /// <param name="message">メッセージ</param>
        protected override void InvokeAction(InteractionMessage message)
        {
            this.AssociatedObject.Close();
        }
    }
}
