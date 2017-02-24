using Livet.Messaging;
using System.Collections.Generic;
using System.Linq;
using WinCap.Services;
using WinCap.ViewModels.Settings;
using WpfUtility.Mvvm;

namespace WinCap.ViewModels
{
    /// <summary>
    /// 設定ウィンドウViewModel
    /// </summary>
    public class SettingsWindowViewModel : WindowViewModel
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
        /// <param name="hookService">フックサービス</param>
        /// <param name="applicationAction">アクションサービス</param>
        public SettingsWindowViewModel(HookService hookService, ApplicationAction applicationAction)
        {
            this.TabItems = new List<TabItemViewModel>
            {
                (this.General = new GeneralViewModel().AddTo(this)),
                (this.Output = new OutputViewModel().AddTo(this)),
                (this.ShortcutKey = new ShortcutKeyViewModel().AddTo(this)),
                (this.VersionInfo = new VersionInfoViewModel().AddTo(this)),
            };
            this.SelectedItem = this.TabItems.FirstOrDefault();
            this.hookService = hookService;
            this.applicationAction = applicationAction;
        }

        #region WindowViewModel members

        /// <summary>
        /// <see cref="Window.ContentRendered"/>イベントが発生したときに呼び出される初期化処理。
        /// </summary>
        protected override void InitializeCore()
        {
            this.hookService.Suspend().AddTo(this);
            this.TabItems.ForEach(x => x.Initialize());
        }

        #endregion

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
                this.SelectedItem.Messenger.Raise(new InteractionMessage(this.SelectedItem.GetErrorProperties().First() + ".Focus"));
                return;
            }
            this.TabItems.ForEach(x => x.Apply());

            this.Close();
            this.applicationAction.CreateShortcut();
        }

        /// <summary>
        /// キャンセル
        /// </summary>
        public void Cancel()
        {
            this.TabItems.ForEach(x => x.Cancel());

            this.Close();
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
