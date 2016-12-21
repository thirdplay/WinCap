using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows.Grasp;
using RM.Friendly.WPFStandardControls;
using System.Collections.Generic;

namespace WinCap.Driver
{
    /// <summary>
    /// 設定ウィンドウを操作する機能を提供します。
    /// </summary>
    public class SettingsWindowDriver
    {
        /// <summary>
        /// ウィンドウコントロールを取得します。
        /// </summary>
        public WindowControl Window { get; private set; }

        /// <summary>
        /// ViewModelを取得します。
        /// </summary>
        public dynamic ViewModel { get; private set; }

        /// <summary>
        /// タブアイテムを取得します。
        /// </summary>
        public WPFListBox TabItems { get; private set; }

        /// <summary>
        /// OKボタンを取得します。
        /// </summary>
        public WPFButtonBase ButtonOk { get; private set; }

        private General _general;
        /// <summary>
        /// 全般タブを取得します。
        /// </summary>
        public General General
        {
            get
            {
                this._general = this._general ?? new General(this.Window, this.ViewModel.General, this.TabItems);
                return this._general;
            }
        }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="windowObject">ウィンドウオブジェクト</param>
        public SettingsWindowDriver(WindowControl windowControl)
        {
            var visualTree = windowControl.VisualTree();
            var logicalTree = windowControl.LogicalTree();

            this.ViewModel = windowControl.Dynamic().DataContext;
            this.Window = windowControl;
            this.TabItems = new WPFListBox(logicalTree.ByType("MetroRadiance.UI.Controls.TabView").ByBinding("TabItems").Single());
            this.ButtonOk = new WPFButtonBase(windowControl.Dynamic()._buttonOk);
        }

        /// <summary>
        /// 設定ウィンドウを閉じます。
        /// </summary>
        public void Close()
        {
            this.Window.Dynamic().Close();
        }
    }

    /// <summary>
    /// 設定ウィンドウの全般タブを操作する機能を提供します。
    /// </summary>
    public class General
    {
        /// <summary>
        /// ViewModelを取得します。
        /// </summary>
        public dynamic ViewModel { get; private set; }

        /// <summary>
        /// タブアイテムを取得します。
        /// </summary>
        public WPFListBox TabItems { get; private set; }

        /// <summary>
        /// スクロール遅延時間を取得します。
        /// </summary>
        public WPFTextBox ScrollDelayTime { get; private set; }

        /// <summary>
        /// キャプチャ遅延時間を取得します。
        /// </summary>
        public WPFTextBox CaptureDelayTime { get; private set; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="windowControl">ウィンドウコントロール</param>
        /// <param name="viewModel">ViewModel</param>
        /// <param name="tabItems">タブアイテム</param>
        public General(WindowControl windowControl, dynamic viewModel, WPFListBox tabItems)
        {
            var visualTree = windowControl.VisualTree();

            this.ViewModel = viewModel;
            this.TabItems = tabItems;
            this.TabItems.EmulateChangeSelectedIndex(0);
            this.ScrollDelayTime = new WPFTextBox(visualTree.ByBinding("ScrollDelayTime").Single());
            this.CaptureDelayTime = new WPFTextBox(visualTree.ByBinding("CaptureDelayTime").Single());
        }

        /// <summary>
        /// プロパティ名のエラーメッセージを取得します。
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        /// <returns>エラーメッセージ</returns>
        public string GetError(string propertyName)
        {
            var errors = (List<string>)this.ViewModel.GetErrors(propertyName);
            if (errors.Count > 0)
            {
                return errors[0];
            }
            return string.Empty;
        }
    }
}
