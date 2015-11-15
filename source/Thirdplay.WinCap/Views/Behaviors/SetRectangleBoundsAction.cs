using Livet.Behaviors.Messaging;
using Livet.Messaging;
using System;
using System.Windows;
using System.Windows.Shapes;
using Thirdplay.WinCap.ViewModels.Messages;

namespace Thirdplay.WinCap.Views.Behaviors
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

            if (setBoundsMessage.Left.HasValue || setBoundsMessage.Top.HasValue)
            {
                var left = setBoundsMessage.Left.HasValue ? setBoundsMessage.Left.Value : this.AssociatedObject.Margin.Left;
                var top = setBoundsMessage.Top.HasValue ? setBoundsMessage.Top.Value : this.AssociatedObject.Margin.Top;
                this.AssociatedObject.Margin = new Thickness(left, top, 0, 0);
            }
        }
    }
}
