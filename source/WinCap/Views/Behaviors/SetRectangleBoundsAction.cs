using Livet.Behaviors.Messaging;
using Livet.Messaging;
using System.Windows.Controls;
using System.Windows.Shapes;
using WinCap.ViewModels.Messages;

namespace WinCap.Views.Behaviors
{
    /// <summary>
    /// 四角形の位置、サイズを設定するアクション
    /// </summary>
    public class SetRectangleBoundsAction : InteractionMessageAction<Rectangle>
    {
        /// <summary>
        /// アクション実行
        /// </summary>
        /// <param name="message">メッセージ</param>
        protected override void InvokeAction(InteractionMessage message)
        {
            var setBoundsMessage = message as SetRectangleBoundsMessage;
            if (setBoundsMessage == null) return;

            if (setBoundsMessage.Width.HasValue) this.AssociatedObject.Width = setBoundsMessage.Width.Value;
            if (setBoundsMessage.Height.HasValue) this.AssociatedObject.Height = setBoundsMessage.Height.Value;
            if (setBoundsMessage.Left.HasValue) Canvas.SetLeft(this.AssociatedObject, setBoundsMessage.Left.Value);
            if (setBoundsMessage.Top.HasValue) Canvas.SetTop(this.AssociatedObject, setBoundsMessage.Top.Value);
        }
    }
}
