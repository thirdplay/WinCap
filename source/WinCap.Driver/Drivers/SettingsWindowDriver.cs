﻿using Codeer.Friendly;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows.Grasp;
using RM.Friendly.WPFStandardControls;
using WinCap.Driver.Drivers;
using WinCap.Driver.ViewModels;

namespace WinCap.Driver.Drivers
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
        public SettingsViewModel ViewModel { get; private set; }

        /// <summary>
        /// タブアイテムを取得します。
        /// </summary>
        public WPFListBox TabItems { get; private set; }

        /// <summary>
        /// OKボタンを取得します。
        /// </summary>
        public WPFButtonBase ButtonOk { get; private set; }

        private General general;
        /// <summary>
        /// 全般タブを取得します。
        /// </summary>
        public General General
        {
            get
            {
                ChangeSelectedIndex(TabItem.General);
                this.general = this.general ?? new General(this.Window, this.ViewModel.General);
                return this.general;
            }
        }

        private Output output;
        /// <summary>
        /// 出力タブを取得します。
        /// </summary>
        public Output Output
        {
            get
            {
                ChangeSelectedIndex(TabItem.Output);
                this.output = this.output ?? new Output(this.Window, this.ViewModel.Output);
                return this.output;
            }
        }

        private ShortcutKey shortcutKey;
        /// <summary>
        /// ショートカットキータブを取得します。
        /// </summary>
        public ShortcutKey ShortcutKey
        {
            get
            {
                ChangeSelectedIndex(TabItem.ShortcutKey);
                this.shortcutKey = this.shortcutKey ?? new ShortcutKey(this.Window, this.ViewModel.ShortcutKey);
                return this.shortcutKey;
            }
        }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="windowObject">ウィンドウオブジェクト</param>
        public SettingsWindowDriver(WindowControl windowControl)
        {
            var logicalTree = windowControl.LogicalTree();

            this.Window = windowControl;
            this.ViewModel = new SettingsViewModel()
            {
                General = new GeneralViewModel(windowControl.Dynamic().DataContext.General),
                Output = new OutputViewModel(windowControl.Dynamic().DataContext.Output),
                ShortcutKey = new ShortcutKeyViewModel(windowControl.Dynamic().DataContext.ShortcutKey),
            };
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

        /// <summary>
        /// 選択中のタブ項目を変更します。
        /// </summary>
        /// <param name="tabItem">タブ項目</param>
        public void ChangeSelectedIndex(TabItem tabItem)
        {
            int index = (int)tabItem;

            if (this.TabItems.SelectedIndex != index)
            {
                this.TabItems.EmulateChangeSelectedIndex(index);
            }
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
        public GeneralViewModel ViewModel { get; private set; }

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
        public General(WindowControl windowControl, GeneralViewModel viewModel)
        {
            var visualTree = windowControl.VisualTree();
            this.ViewModel = viewModel;
            this.ScrollDelayTime = new WPFTextBox(visualTree.ByBinding("ScrollDelayTime").Single());
            this.CaptureDelayTime = new WPFTextBox(visualTree.ByBinding("CaptureDelayTime").Single());
        }

        /// <summary>
        /// プロパティ名のエラーメッセージを取得します。
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        /// <returns>エラーメッセージ</returns>
        public string GetError(string propertyName) => this.ViewModel.GetError(propertyName);
    }

    /// <summary>
    /// 設定ウィンドウの出力タブを操作する機能を提供します。
    /// </summary>
    public class Output
    {
        /// <summary>
        /// ViewModelを取得します。
        /// </summary>
        public OutputViewModel ViewModel { get; private set; }

        /// <summary>
        /// 出力フォルダを取得します。
        /// </summary>
        public WPFTextBox OutputFolder { get; private set; }

        /// <summary>
        /// 画像を自動保存するかどうかを示す値を取得します。
        /// </summary>
        public WPFToggleButton IsAutoSaveImage { get; private set; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="windowControl">ウィンドウコントロール</param>
        /// <param name="viewModel">ViewModel</param>
        public Output(WindowControl windowControl, OutputViewModel viewModel)
        {
            var visualTree = windowControl.VisualTree();
            this.ViewModel = viewModel;
            this.OutputFolder = new WPFTextBox(visualTree.ByBinding("OutputFolder").Single());
            this.IsAutoSaveImage = new WPFToggleButton(visualTree.ByBinding("IsAutoSaveImage").Single());
        }

        /// <summary>
        /// プロパティ名のエラーメッセージを取得します。
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        /// <returns>エラーメッセージ</returns>
        public string GetError(string propertyName) => this.ViewModel.GetError(propertyName);
    }

    /// <summary>
    /// 設定ウィンドウのショートカットキータブを操作する機能を提供します。
    /// </summary>
    public class ShortcutKey
    {
        /// <summary>
        /// ViewModelを取得します。
        /// </summary>
        public ShortcutKeyViewModel ViewModel { get; private set; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="windowControl">ウィンドウコントロール</param>
        /// <param name="viewModel">ViewModel</param>
        public ShortcutKey(WindowControl windowControl, ShortcutKeyViewModel viewModel)
        {
            var visualTree = windowControl.VisualTree();
            this.ViewModel = viewModel;
            var fullScreen = visualTree.ByBinding("FullScreen").Single();

            //var current = (int[])fullScreen.Dynamic().Current;
            //System.Diagnostics.Debug.WriteLine("Current:" + current[0]);
        }

        /// <summary>
        /// プロパティ名のエラーメッセージを取得します。
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        /// <returns>エラーメッセージ</returns>
        public string GetError(string propertyName) => this.ViewModel.GetError(propertyName);
    }
}