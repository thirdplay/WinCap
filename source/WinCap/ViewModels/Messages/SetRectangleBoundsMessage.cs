using Livet.Messaging;
using System.Drawing;
using System.Windows;

namespace WinCap.ViewModels.Messages
{
    /// <summary>
    /// 四角形の位置とサイズを設定するメッセージ
    /// </summary>
    public class SetRectangleBoundsMessage : InteractionMessage
    {
        #region Bounds 依存関係プロパティ
        public Rectangle Bounds
        {
            get { return (Rectangle)this.GetValue(BoundsProperty); }
            set { this.SetValue(BoundsProperty, value); }
        }
        public static readonly DependencyProperty BoundsProperty =
            DependencyProperty.Register(nameof(Bounds), typeof(Rectangle), typeof(SetRectangleBoundsMessage), new UIPropertyMetadata(null));
        #endregion

        protected override Freezable CreateInstanceCore()
        {
            return new SetRectangleBoundsMessage
            {
                MessageKey = this.MessageKey,
                Bounds = this.Bounds
            };
        }
    }
}
