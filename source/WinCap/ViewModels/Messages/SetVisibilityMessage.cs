using Livet.Messaging;
using System.Windows;

namespace WinCap.ViewModels.Messages
{
    /// <summary>
    /// 可視性を設定するメッセージ
    /// </summary>
    public class SetVisibilityMessage : InteractionMessage
    {
        #region Visibility 依存関係プロパティ
        public Visibility Visibility
        {
            get { return (Visibility)GetValue(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }
        public static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.Register(nameof(Visibility), typeof(Visibility), typeof(SetVisibilityMessage), new UIPropertyMetadata(null));
        #endregion

        protected override Freezable CreateInstanceCore()
        {
            return new SetVisibilityMessage
            {
                MessageKey = this.MessageKey,
                Visibility = this.Visibility
            };
        }
    }
}
