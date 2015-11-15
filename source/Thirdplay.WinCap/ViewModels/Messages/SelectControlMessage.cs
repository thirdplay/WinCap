using Livet.Messaging;
using System;
using System.Windows;

namespace WinCap.ViewModels.Messages
{
    /// <summary>
    /// 選択メッセージ
    /// </summary>
    public class SelectControlMessage : InteractionMessage
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
            DependencyProperty.Register(nameof(Handle), typeof(IntPtr), typeof(SelectControlMessage), new UIPropertyMetadata(null));
        #endregion

        protected override Freezable CreateInstanceCore()
        {
            return new SelectControlMessage
            {
                MessageKey = this.MessageKey,
                Handle = this.Handle
            };
        }
    }
}
