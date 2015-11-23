using Livet.Messaging;
using System;
using System.Windows;

namespace WinCap.ViewModels.Messages
{
    /// <summary>
    /// 選択メッセージ
    /// </summary>
    public class SelectedControlMessage : InteractionMessage
    {
        #region Handle 依存関係プロパティ
        /// <summary>
        /// ウィンドウハンドル
        /// </summary>
        public IntPtr Handle
        {
            get { return (IntPtr)this.GetValue(HandleProperty); }
            set { this.SetValue(HandleProperty, value); }
        }
        public static readonly DependencyProperty HandleProperty =
            DependencyProperty.Register(nameof(Handle), typeof(IntPtr), typeof(SelectedControlMessage), new UIPropertyMetadata(null));
        #endregion

        protected override Freezable CreateInstanceCore()
        {
            return new SelectedControlMessage
            {
                MessageKey = this.MessageKey,
                Handle = this.Handle
            };
        }
    }
}
