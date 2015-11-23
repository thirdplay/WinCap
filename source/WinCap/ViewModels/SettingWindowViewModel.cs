using Livet;
using Livet.Messaging;
using MetroTrilithon.Mvvm;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using WinCap.Models;
using WinCap.Utilities.Drawing;
using WinCap.ViewModels.Messages;
using WinCap.Win32;

namespace WinCap.ViewModels
{
    /// <summary>
    /// 設定ウィンドウViewModel
    /// </summary>
    public class SettingWindowViewModel : ViewModel
    {
        /// <summary>
        /// タイトル
        /// </summary>
        public string Title => ProductInfo.Title;

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
            Console.WriteLine("SettingWindowViewModel.InitializeSettingWindowViewModel");
        }
    }
}
