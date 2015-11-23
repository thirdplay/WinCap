using System;
using WinCap.Models;

namespace WinCap.Views
{
    /// <summary>
    /// ControlSelectionWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ControlSelectionWindow
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ControlSelectionWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// コントロール選択時に発生するイベント
        /// </summary>
        public event EventHandler<SelectedEventArgs> Selected;

        /// <summary>
        /// コントロール選択イベントを呼び出します。
        /// </summary>
        /// <param name="handle">ウィンドウハンドル</param>
        public void OnSelected(IntPtr handle)
        {
            this.Selected?.Invoke(this, new SelectedEventArgs(handle));
        }
    }
}
