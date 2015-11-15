using Livet.Messaging;
using System.Drawing;
using System.Windows;

namespace Thirdplay.WinCap.ViewModels.Messages
{
    /// <summary>
    /// 余白を設定するメッセージ
    /// </summary>
    public class SetMarginMessage : InteractionMessage
    {
        #region Left 依存関係プロパティ
        public double? Left
        {
            get { return (double?)this.GetValue(LeftProperty); }
            set { this.SetValue(LeftProperty, value); }
        }
        public static readonly DependencyProperty LeftProperty =
            DependencyProperty.Register(nameof(Left), typeof(double?), typeof(SetMarginMessage), new UIPropertyMetadata(null));
        #endregion

        #region Top 依存関係プロパティ
        public double? Top
        {
            get { return (double?)this.GetValue(TopProperty); }
            set { this.SetValue(TopProperty, value); }
        }
        public static readonly DependencyProperty TopProperty =
            DependencyProperty.Register(nameof(Top), typeof(double?), typeof(SetMarginMessage), new UIPropertyMetadata(null));
        #endregion

        #region Right 依存関係プロパティ
        public double? Right
        {
            get { return (double?)this.GetValue(RightProperty); }
            set { this.SetValue(RightProperty, value); }
        }
        public static readonly DependencyProperty RightProperty =
            DependencyProperty.Register(nameof(Right), typeof(double?), typeof(SetMarginMessage), new UIPropertyMetadata(null));
        #endregion

        #region Bottom 依存関係プロパティ
        public double? Bottom
        {
            get { return (double?)this.GetValue(BottomProperty); }
            set { this.SetValue(BottomProperty, value); }
        }
        public static readonly DependencyProperty BottomProperty =
            DependencyProperty.Register(nameof(Bottom), typeof(double?), typeof(SetMarginMessage), new UIPropertyMetadata(null));
        #endregion

        protected override Freezable CreateInstanceCore()
        {
            return new SetMarginMessage
            {
                MessageKey = this.MessageKey,
                Left = this.Left,
                Top = this.Top,
                Right = this.Right,
                Bottom = this.Bottom
            };
        }
    }
}
