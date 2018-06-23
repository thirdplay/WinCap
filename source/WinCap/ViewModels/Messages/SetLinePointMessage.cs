using Livet.Messaging;
using System.Windows;

namespace WinCap.ViewModels.Messages
{
    /// <summary>
    /// 四角形の位置とサイズを設定するメッセージ
    /// </summary>
    public class SetLinePointMessage : InteractionMessage
    {
        #region X1 依存関係プロパティ
        public double? X1
        {
            get { return (double?)this.GetValue(X1Property); }
            set { this.SetValue(X1Property, value); }
        }
        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register(nameof(X1), typeof(double?), typeof(SetLinePointMessage), new UIPropertyMetadata(null));
        #endregion

        #region Y1 依存関係プロパティ
        public double? Y1
        {
            get { return (double?)this.GetValue(Y1Property); }
            set { this.SetValue(Y1Property, value); }
        }
        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register(nameof(Y1), typeof(double?), typeof(SetLinePointMessage), new UIPropertyMetadata(null));
        #endregion

        #region X2 依存関係プロパティ
        public double? X2
        {
            get { return (double?)this.GetValue(X2Property); }
            set { this.SetValue(X2Property, value); }
        }
        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register(nameof(X2), typeof(double?), typeof(SetLinePointMessage), new UIPropertyMetadata(null));
        #endregion

        #region Y2 依存関係プロパティ
        public double? Y2
        {
            get { return (double?)this.GetValue(Y2Property); }
            set { this.SetValue(Y2Property, value); }
        }
        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register(nameof(Y2), typeof(double?), typeof(SetLinePointMessage), new UIPropertyMetadata(null));
        #endregion

        protected override Freezable CreateInstanceCore()
        {
            return new SetLinePointMessage
            {
                MessageKey = this.MessageKey,
                X1 = this.X1,
                Y1 = this.Y1,
                X2 = this.X2,
                Y2 = this.Y2
            };
        }
    }
}
