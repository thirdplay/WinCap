using Livet.Messaging;
using System.Windows;

namespace WinCap.ViewModels.Messages
{
    /// <summary>
    /// ダイアログ結果を設定するメッセージ
    /// </summary>
    public class SetDialogResultMessage : InteractionMessage
    {
        #region DialogResult 依存関係プロパティ
        public bool DialogResult
        {
            get { return (bool)GetValue(DialogResultProperty); }
            set { SetValue(DialogResultProperty, value); }
        }
        public static readonly DependencyProperty DialogResultProperty =
            DependencyProperty.Register(nameof(DialogResult), typeof(bool), typeof(SetDialogResultMessage), new UIPropertyMetadata(null));
        #endregion

        protected override Freezable CreateInstanceCore()
        {
            return new SetDialogResultMessage
            {
                MessageKey = this.MessageKey,
                DialogResult = this.DialogResult
            };
        }
    }
}
