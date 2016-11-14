using Livet;
using Livet.Messaging;
using System.Collections.Generic;
using System.Linq;
using WinCap.Util.Mvvm;
using WinCap.ViewModels.Messages;
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

        /// <summary>
        /// ショートカットキー設定ViewModel
        /// </summary>
        public VersionInfoViewModel VersionInfo { get; } = new VersionInfoViewModel();
        #endregion

        /// <summary>
        /// タブ項目リスト
        /// </summary>
        public List<SettingsBaseViewModel> TabItems { get; set; }

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
            this.TabItems = new List<SettingsBaseViewModel>
            {
                (this.General = new GeneralViewModel().AddTo(this)),
                (this.Output = new OutputViewModel().AddTo(this)),
                (this.ShortcutKey = new ShortcutKeyViewModel().AddTo(this)),
                (this.VersionInfo = new VersionInfoViewModel().AddTo(this)),
            };
            this.SelectedItem = this.TabItems.FirstOrDefault();
        }

        /// <summary>
		/// <see cref="Window.ContentRendered"/>イベントが発生したときに
        /// Livet インフラストラクチャによって呼び出されます。
        /// </summary>
        public void Initialize()
        {
            this.TabItems.ForEach(x => x.Initialize());
        }

        /// <summary>
        /// OK
        /// </summary>
        public void Ok()
        {
            this.TabItems.ForEach(x => x.Validate());
            var tabItem = this.TabItems.Where(x => x.HasErrors).FirstOrDefault();
            if (tabItem != null)
            {
                this.SelectedItem = tabItem;
                this.SelectedItem.Messenger.Raise(new InteractionMessage(this.SelectedItem.FirstErrorPropertyName + ".Focus"));
                return;
            }
            this.TabItems.ForEach(x => x.Apply());

            this.Messenger.Raise(new SetDialogResultMessage(){
                MessageKey = "Window.DialogResult",
                DialogResult = true
            });
        }

        /// <summary>
        /// キャンセル
        /// </summary>
        public void Cancel()
        {
            this.TabItems.ForEach(x => x.Cancel());

            this.Messenger.Raise(new SetDialogResultMessage()
            {
                MessageKey = "Window.DialogResult",
                DialogResult = false
            });
        }
    }
}
