using Livet;
using Livet.Messaging;
using System.Collections.Generic;
using System.Linq;
using WinCap.Serialization;
using WinCap.Services;
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

        /// <summary>
        /// ショートカットキー設定ViewModel
        /// </summary>
        public VersionInfoViewModel VersionInfo { get; } = new VersionInfoViewModel();
        #endregion

        /// <summary>
        /// フックサービス
        /// </summary>
        private readonly HookService hookService;

        /// <summary>
        /// アプリケーションアクション
        /// </summary>
        private readonly ApplicationAction applicationAction;

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
        /// ダイアログ結果を取得します。
        /// </summary>
        public bool DialogResult { get; private set; }

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
            this.hookService = Application.Instance.HookService;
            this.applicationAction = Application.Instance.ApplicationAction;
        }

        /// <summary>
		/// <see cref="Window.ContentRendered"/>イベントが発生したときに
        /// Livet インフラストラクチャによって呼び出されます。
        /// </summary>
        public void Initialize()
        {
            this.hookService.Suspend().AddTo(this);
            this.applicationAction.DeregisterActions();
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
                this.SelectedItem.Messenger.Raise(new InteractionMessage(this.SelectedItem.GetErrorPropertyName() + ".Focus"));
                return;
            }
            this.TabItems.ForEach(x => x.Apply());

            this.DialogResult = true;
            this.Messenger.Raise(new InteractionMessage("Window.Close"));
        }

        /// <summary>
        /// キャンセル
        /// </summary>
        public void Cancel()
        {
            this.TabItems.ForEach(x => x.Cancel());

            this.DialogResult = false;
            this.Messenger.Raise(new InteractionMessage("Window.Close"));
        }

        /// <summary>
        /// このインスタンスによって使用されているリソースを全て破棄します。
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
