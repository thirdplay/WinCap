using Livet.Messaging;
using System.Windows;

namespace WinCap.ViewModels.Messages
{
    /// <summary>
    /// ウィンドウの位置とサイズを設定するメッセージ
    /// </summary>
    public class SetWindowBoundsMessage : InteractionMessage
    {
        #region Left 依存関係プロパティ
        public double? Left
        {
            get { return (double?)this.GetValue(LeftProperty); }
            set { this.SetValue(LeftProperty, value); }
        }
        public static readonly DependencyProperty LeftProperty =
            DependencyProperty.Register(nameof(Left), typeof(double?), typeof(SetWindowBoundsMessage), new UIPropertyMetadata(null));
        #endregion

        #region Top 依存関係プロパティ
        public double? Top
        {
            get { return (double?)this.GetValue(TopProperty); }
            set { this.SetValue(TopProperty, value); }
        }
        public static readonly DependencyProperty TopProperty =
            DependencyProperty.Register(nameof(Top), typeof(double?), typeof(SetWindowBoundsMessage), new UIPropertyMetadata(null));
        #endregion

        #region Width 依存関係プロパティ
        public double? Width
        {
            get { return (double?)this.GetValue(WidthProperty); }
            set { this.SetValue(WidthProperty, value); }
        }
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(nameof(Width), typeof(double?), typeof(SetWindowBoundsMessage), new UIPropertyMetadata(null));
        #endregion

        #region Height 依存関係プロパティ
        public double? Height
        {
            get { return (double?)this.GetValue(HeightProperty); }
            set { this.SetValue(HeightProperty, value); }
        }
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register(nameof(Height), typeof(double?), typeof(SetWindowBoundsMessage), new UIPropertyMetadata(null));
        #endregion

        protected override Freezable CreateInstanceCore()
        {
            return new SetWindowBoundsMessage
            {
                MessageKey = this.MessageKey,
                Left = this.Left,
                Top = this.Top,
                Width = this.Width,
                Height = this.Height
            };
        }
    }
}
