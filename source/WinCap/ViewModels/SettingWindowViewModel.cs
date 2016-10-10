using Livet;
using Livet.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using WinCap.Models;
using WinCap.Properties;
using WinCap.Util.Mvvm;
using WinCap.ViewModels.Settings;

namespace WinCap.ViewModels
{
    /// <summary>
    /// 設定ウィンドウViewModel
    /// </summary>
    public class SettingWindowViewModel : ViewModel
    {
        #region TanItems ViewModel
        /// <summary>
        /// 一般設定ViewModel
        /// </summary>
        public GeneralViewModel General { get; } = new GeneralViewModel();

        /// <summary>
        /// キャプチャー設定ViewModel
        /// </summary>
        public CaptureViewModel Capture { get; } = new CaptureViewModel();

        /// <summary>
        /// 出力設定ViewModel
        /// </summary>
        public OutputViewModel Output { get; } = new OutputViewModel();

        /// <summary>
        /// ショートカットキー設定ViewModel
        /// </summary>
        public ShortcutKeyViewModel ShortcutKey { get; } = new ShortcutKeyViewModel();
        #endregion

        //#region SelectedItem 変更通知プロパティ
        //private TabItemViewModel _SelectedItem;
        ///// <summary>
        ///// 選択項目を取得、設定します。
        ///// </summary>
        //public TabItemViewModel SelectedItem
        //{
        //    get { return this._SelectedItem; }
        //    set
        //    {
        //        if (this._SelectedItem != value)
        //        {
        //            this._SelectedItem = value;
        //            this.RaisePropertyChanged();
        //        }
        //    }
        //}

        //#endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SettingWindowViewModel()
        {
        }

        /// <summary>
		/// <see cref="Window.ContentRendered"/>イベントが発生したときに
        /// Livet インフラストラクチャによって呼び出されます。
        /// </summary>
        public void Initialize()
        {
        }

        public ICommand OkCommand { get; } = new ViewModelCommand(() =>
        {
            Console.WriteLine("Ok");
        });
        public ICommand CancelCommand { get; } = new ViewModelCommand(() =>
        {
            Console.WriteLine("Cancel");
        });
    }
}
