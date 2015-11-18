using Livet;
using MetroTrilithon.Lifetime;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Interop;
using WinCap.Models.Settings;

namespace WinCap.Models
{
    /// <summary>
    /// ホットキーを取り付け、分離する機能を提供します。
    /// </summary>
    public sealed class HotkeyService : NotificationObject, IDisposableHolder
    {
        #region フィールド
        /// <summary>
        /// 基本CompositeDisposable
        /// </summary>
        private readonly LivetCompositeDisposable compositeDisposable = new LivetCompositeDisposable();

        /// <summary>
        /// ホットキーリスト
        /// </summary>
        private static Dictionary<HotkeyId, Hotkey> hotkeies = new Dictionary<HotkeyId, Hotkey>();
        #endregion

        #region プロパティ
        /// <summary>
        /// 現在のサービスを取得します。
        /// </summary>
        public static HotkeyService Current { get; } = new HotkeyService();
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private HotkeyService() { }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="window">ウィンドウ</param>
        public void Initialize(Window window)
        {
            if (!window.IsLoaded)
            {
                Observable.FromEventPattern(window, nameof(window.Loaded))
                    .Subscribe(_ => createHotkey(window))
                    .AddTo(this);
            }
            else
            {
                createHotkey(window);
            }
        }

        /// <summary>
        /// ホットキーを取り付けます。
        /// </summary>
        public void Attach()
        {
            hotkeies[HotkeyId.ScreenWhole].Attach(HotkeySetting.ScreenWholeKey);
            hotkeies[HotkeyId.ActiveWindow].Attach(HotkeySetting.ActiveWindowKey);
            hotkeies[HotkeyId.SelectControl].Attach(HotkeySetting.SelectControlKey);
            hotkeies[HotkeyId.PageWhole].Attach(HotkeySetting.PageWholeKey);
        }

        /// <summary>
        /// ホットキーを分離します。
        /// </summary>
        public void Detach()
        {
            foreach (Hotkey hotkey in hotkeies.Values)
            {
                hotkey.Detach();
            }
        }

        /// <summary>
        /// ホットキーを生成します。
        /// </summary>
        /// <param name="window">ウィンドウ</param>
        private void createHotkey(Window window)
        {
            IntPtr handle = new WindowInteropHelper(window).Handle;
            hotkeies.Add(HotkeyId.ScreenWhole, new Hotkey(handle, (int)HotkeyId.ScreenWhole, CaptureService.Current.CaptureScreenWhole).AddTo(this));
            hotkeies.Add(HotkeyId.ActiveWindow, new Hotkey(handle, (int)HotkeyId.ActiveWindow, CaptureService.Current.CaptureActiveWindow).AddTo(this));
            hotkeies.Add(HotkeyId.SelectControl, new Hotkey(handle, (int)HotkeyId.SelectControl, CaptureService.Current.CaptureSelectControl).AddTo(this));
            hotkeies.Add(HotkeyId.PageWhole, new Hotkey(handle, (int)HotkeyId.PageWhole, CaptureService.Current.CapturePageWhole).AddTo(this));
            Attach();
        }

        #region IDisposableHoloder members
        ICollection<IDisposable> IDisposableHolder.CompositeDisposable => this.compositeDisposable;

        /// <summary>
        /// このインスタンスによって使用されているリソースを全て破棄します。
        /// </summary>
        public void Dispose()
        {
            this.compositeDisposable.Dispose();
        }
        #endregion
    }
}
