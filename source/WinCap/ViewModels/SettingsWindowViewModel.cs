using Livet;
using Livet.Messaging;
using MetroRadiance.Utilities;
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
        /// コンストラクタ
        /// </summary>
        public SettingsWindowViewModel()
        {
            Disposable.Create(() => LocalSettingsProvider.Instance.SaveAsync().Wait()).AddTo(this);
        }

        /// <summary>
		/// <see cref="Window.ContentRendered"/>イベントが発生したときに
        /// Livet インフラストラクチャによって呼び出されます。
        /// </summary>
        public void Initialize()
        {
            this.General.Initialize();
            this.Output.Initialize();
            this.ShortcutKey.Initialize();
        }

        /// <summary>
        /// OK
        /// </summary>
        public void Ok()
        {
            this.General.Apply();
            this.Output.Apply();
            this.ShortcutKey.Apply();

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
