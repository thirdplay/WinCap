using Livet;
using System;
using System.Collections.Generic;
using WinCap.Utilities.Lifetime;

namespace WinCap.Views
{
    /// <summary>
    /// SelectWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectWindow /*: IDisposableHolder*/
    {
        #region フィールド
        /// <summary>
        /// 基本CompositeDisposable
        /// </summary>
        //private readonly LivetCompositeDisposable compositeDisposable = new LivetCompositeDisposable();
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SelectWindow()
        {
            InitializeComponent();

            //// スクリーン全体の座標を求める
            //Point point = new Point();
            //foreach (Screen screen in Screen.AllScreens)
            //{
            //    point.X = Math.Min(point.X, screen.Bounds.Left);
            //    point.Y = Math.Min(point.Y, screen.Bounds.Top);
            //}

            //// 画面全体にキャプチャしたスクリーンを表示する
            //this.Left = point.X;
            //this.Top = point.Y;
            //this.Width = screenBmp_.Width;
            //this.Height = screenBmp_.Height;
        }

        // TODO:実装時に下記をビヘイビアへ移動する
        // ※EventTrigger
        public event EventHandler Selected = null;
        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Selected?.Invoke(sender, e);
        }

        //#region IDisposableHoloder members
        //ICollection<IDisposable> IDisposableHolder.CompositeDisposable => this.compositeDisposable;

        ///// <summary>
        ///// このインスタンスによって使用されているリソースを全て破棄する。
        ///// </summary>
        //public void Dispose()
        //{
        //    this.compositeDisposable.Dispose();
        //}

        ///// <summary>
        ///// クローズイベント
        ///// </summary>
        ///// <param name="e">イベント引数</param>
        //protected override void OnClosed(EventArgs e)
        //{
        //    base.OnClosed(e);
        //    ((IDisposable)this).Dispose();
        //}
        //#endregion
    }
}
