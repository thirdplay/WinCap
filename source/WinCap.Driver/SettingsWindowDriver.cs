using Codeer.Friendly;
using Codeer.Friendly.Windows.Grasp;
using RM.Friendly.WPFStandardControls;

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
        public object ViewModel { get; private set; }

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
                if (this._general == null)
                {
                    this._general = new General(this.Window, this.ViewModel, this.TabItems);
                }
                return this._general;
            }
        }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="windowObject">ウィンドウオブジェクト</param>
        public SettingsWindowDriver(dynamic windowObject)
        {
            var windowControl = new WindowControl(windowObject);
            var visualTree = windowControl.VisualTree();
            var logicalTree = windowControl.LogicalTree();

            this.ViewModel = windowObject.DataContext;
            this.Window = windowControl;
            this.TabItems = new WPFListBox(logicalTree.ByType("MetroRadiance.UI.Controls.TabView").ByBinding("TabItems").Single());
            this.ButtonOk = new WPFButtonBase(windowObject._buttonOk);
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
        /// スクロールの遅延時間を取得します。
        /// </summary>
        public WPFTextBox ScrollDelayTime { get; private set; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="windowControl">ウィンドウコントロール</param>
        /// <param name="viewModel">ViewModel</param>
        /// <param name="tabItems">タブアイテム</param>
        public General(WindowControl windowControl, dynamic viewModel, WPFListBox tabItems)
        {
            var visualTree = windowControl.VisualTree();

            this.ViewModel = viewModel.General;
            this.TabItems = tabItems;
            this.TabItems.EmulateChangeSelectedIndex(0);
            this.ScrollDelayTime = new WPFTextBox(visualTree.ByBinding("ScrollDelayTime").Single());
        }

        /// <summary>
        /// プロパティ名のエラーメッセージを取得します。
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        /// <returns>エラーメッセージ</returns>
        public string GetError(string propertyName)
        {
            var errors = this.ViewModel.GetErrors(propertyName).CodeerFriendlyAppVar.Core;
            if (errors.Count > 0)
            {
                return errors[0];
            }
            return string.Empty;
        }
    }
}
