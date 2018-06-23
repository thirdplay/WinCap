using Livet.Behaviors.Messaging;
using Livet.Messaging;
using System.Windows.Shapes;
using WinCap.ViewModels.Messages;

namespace WinCap.Views.Behaviors
{
    /// <summary>
    /// 直線の始点と終点を設定するアクション
    /// </summary>
    public class SetLinePointAction : InteractionMessageAction<Line>
    {
        /// <summary>
        /// アクション実行
        /// </summary>
        /// <param name="message">メッセージ</param>
        protected override void InvokeAction(InteractionMessage message)
        {
            var setBoundsMessage = message as SetLinePointMessage;
            if (setBoundsMessage == null) return;

            if (setBoundsMessage.X1.HasValue) this.AssociatedObject.X1 = setBoundsMessage.X1.Value;
            if (setBoundsMessage.Y1.HasValue) this.AssociatedObject.Y1 = setBoundsMessage.Y1.Value;
            if (setBoundsMessage.X2.HasValue) this.AssociatedObject.X2 = setBoundsMessage.X2.Value;
            if (setBoundsMessage.Y2.HasValue) this.AssociatedObject.Y2 = setBoundsMessage.Y2.Value;
        }
    }
}
