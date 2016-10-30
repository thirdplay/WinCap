using Livet;
using Livet.Messaging;
using MetroRadiance.Utilities;
using System.Collections.Generic;
using System.Linq;
using WinCap.Serialization;
using WinCap.Util.Mvvm;
using WinCap.ViewModels.Settings;

namespace WinCap.ViewModels
{
    /// <summary>
    /// 設定ウィンドウViewModel
    /// </summary>
    public class SettingsWindowViewModel : ViewModel
    {
        #region ViewModels
        /// <summary>
        /// 一般設定ViewModel
        /// </summary>
        public GeneralViewModel General { get; } = new GeneralViewModel();

        /// <summary>
        /// 出力設定ViewModel
        /// </summary>
        public OutputViewModel Output { get; } = new OutputViewModel();

        /// <summary>
        /// ショートカットキー設定ViewModel
        /// </summary>
        public ShortcutKeyViewModel ShortcutKey { get; } = new ShortcutKeyViewModel();
        #endregion

        /// <summary>
        /// タブ項目リスト
        /// </summary>
        public List<TabItemViewModel> TabItems { get; set; }

        #region SelectedItem 変更通知プロパティ
        private TabItemViewModel _SelectedItem;
        /// <summary>
        /// 選択中のタブ項目を取得します。
        /// </summary>
        public TabItemViewModel SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                if (_SelectedItem != value)
                {
                    _SelectedItem = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SettingsWindowViewModel()
        {
            this.TabItems = new List<TabItemViewModel>
            {
                (this.General = new GeneralViewModel().AddTo(this)),
                (this.Output = new OutputViewModel().AddTo(this)),
                (this.ShortcutKey = new ShortcutKeyViewModel().AddTo(this)),
            };
            this.SelectedItem = this.TabItems.FirstOrDefault();
        }

        /// <summary>
		/// <see cref="Window.ContentRendered"/>イベントが発生したときに
        /// Livet インフラストラクチャによって呼び出されます。
        /// </summary>
        public void Initialize()
        {
            foreach (ISettingsViewModel vm in TabItems)
            {
                vm.Initialize();
            }
        }

        /// <summary>
        /// OK
        /// </summary>
        public void Ok()
        {
            foreach (ISettingsViewModel vm in TabItems)
            {
                if (!vm.Validate())
                {
                    this.SelectedItem = vm as TabItemViewModel;
                    return;
                }
                vm.Apply();
            }

            LocalSettingsProvider.Instance.SaveAsync().Wait();

            this.Messenger.Raise(new InteractionMessage("Window.Close"));
        }

        /// <summary>
        /// キャンセル
        /// </summary>
        public void Cancel()
        {
            this.General.Cancel();
            this.Output.Cancel();
            this.ShortcutKey.Cancel();

            this.Messenger.Raise(new InteractionMessage("Window.Close"));
        }
    }
}
