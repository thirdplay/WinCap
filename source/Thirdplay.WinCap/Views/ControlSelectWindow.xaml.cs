using System;
using Thirdplay.WinCap.Models;

namespace Thirdplay.WinCap.Views
{
    /// <summary>
    /// ControlSelectWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ControlSelectWindow
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ControlSelectWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// コントロール選択時に発生するイベント
        /// </summary>
        public event EventHandler<SelectedEventArgs> Selected;

        /// <summary>
        /// コントロールを選択する。
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        public void Select(IntPtr handle)
        {
            this.Selected?.Invoke(this, new SelectedEventArgs(handle));
        }
    }
}
