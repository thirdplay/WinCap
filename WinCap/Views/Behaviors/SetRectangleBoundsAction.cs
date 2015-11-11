using Livet.Behaviors.Messaging;
using Livet.Messaging;
using System.Windows;
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
            var setRectangleBoundsMessage = message as SetRectangleBoundsMessage;
            if (setRectangleBoundsMessage == null) return;
            if (setRectangleBoundsMessage.Bounds == null) return;

            var bounds = setRectangleBoundsMessage.Bounds;
            this.AssociatedObject.Margin = new Thickness(bounds.Left, bounds.Top, bounds.Width - bounds.Right, bounds.Height - bounds.Bottom);
            this.AssociatedObject.Width = bounds.Width;
            this.AssociatedObject.Height = bounds.Height;
        }
    }
}
