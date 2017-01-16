using Livet.Behaviors;
using System.Windows;
using System.Windows.Interactivity;

namespace WinCap.Views.Behaviors
{
    /// <summary>
    /// 指定した引数が1つだけ存在するメソッドを呼び出すアクション
    /// </summary>
    public class CallMethodWithArgumentAction : TriggerAction<DependencyObject>
    {
        #region フィールド
        /// <summary>
        /// 引数が1つだけ存在するメソッドを直接バインディングするためのクラス
        /// </summary>
        private MethodBinderWithArgument binderWithArgument = new MethodBinderWithArgument();
        #endregion

        #region MethodTarget 依存関係プロパティ
        /// <summary>
        /// メソッドを呼び出すオブジェクト
        /// </summary>
        public object MethodTarget
        {
            get { return this.GetValue(MethodTargetProperty); }
            set { this.SetValue(MethodTargetProperty, value); }
        }
        public static readonly DependencyProperty MethodTargetProperty =
            DependencyProperty.Register(nameof(MethodTarget), typeof(object), typeof(CallMethodWithArgumentAction), new UIPropertyMetadata(null));
        #endregion

        #region MethodName 依存関係プロパティ
        /// <summary>
        /// 呼び出すメソッド名
        /// </summary>
        public string MethodName
        {
            get { return (string)this.GetValue(MethodNameProperty); }
            set { this.SetValue(MethodNameProperty, value); }
        }
        public static readonly DependencyProperty MethodNameProperty =
            DependencyProperty.Register(nameof(MethodName), typeof(string), typeof(CallMethodWithArgumentAction), new UIPropertyMetadata(null));
        #endregion

        /// <summary>
        /// アクション実行
        /// </summary>
        /// <param name="parameter">パラメータ</param>
        protected override void Invoke(object parameter)
        {
            this.binderWithArgument.Invoke(this.MethodTarget, this.MethodName, parameter);
        }
    }
}
