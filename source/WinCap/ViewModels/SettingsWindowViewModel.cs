using Livet;
using Livet.Commands;
using Livet.Messaging;
using MetroRadiance.Utilities;
using System.Windows.Input;
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
        }

        #region OK Command
        private ICommand _OkCommand;
        public ICommand OkCommand { get { return _OkCommand = _OkCommand ?? new ViewModelCommand(Ok); } }
        private void Ok()
        {
            this.General.Apply();
            this.Output.Apply();

            this.Messenger.Raise(new InteractionMessage("Window.Close"));
        }
        #endregion

        #region Cencel Command
        private ICommand _CancelCommand;
        public ICommand CancelCommand { get { return _CancelCommand = _CancelCommand ?? new ViewModelCommand(Cancel); } }
        private void Cancel()
        {
            this.General.Cancel();
            this.Messenger.Raise(new InteractionMessage("Window.Close"));
        }
        #endregion
    }
}
