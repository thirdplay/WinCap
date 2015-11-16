using Livet;
using MetroTrilithon.Lifetime;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using WinCap.Models.Settings;
using WinCap.Win32;

namespace WinCap.Models
{
    /// <summary>
    /// ホットキーサービス
    /// </summary>
    public sealed class HotkeyService : NotificationObject, IDisposableHolder
    {
        #region フィールド
        /// <summary>
        /// 基本CompositeDisposable
        /// </summary>
        private readonly LivetCompositeDisposable compositeDisposable = new LivetCompositeDisposable();

        /// <summary>
        /// ウィンドウのハンドル
        /// </summary>
        private IntPtr handle = IntPtr.Zero;

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
                    .Subscribe(_ =>
                    {
                        hookWndProc(window);
                        Attach();
                    })
                    .AddTo(this);
            }
            else
            {
                hookWndProc(window);
                Attach();
            }
        }

        /// <summary>
        /// ホットキーを取り付けます。
        /// </summary>
        public void Attach()
        {
            hotkeies[HotkeyId.ScreenWhole].Attach(HotkeySettings.ScreenWholeKey);
            hotkeies[HotkeyId.ActiveWindow].Attach(HotkeySettings.ActiveWindowKey);
            hotkeies[HotkeyId.SelectControl].Attach(HotkeySettings.SelectControlKey);
            hotkeies[HotkeyId.PageWhole].Attach(HotkeySettings.PageWholeKey);
        }

        /// <summary>
        /// ホットキーを分離します。
        /// </summary>
        public void Detach()
        {
            foreach(Hotkey hotkey in hotkeies.Values)
            {
                hotkey.Detach();
            }
        }

        /// <summary>
        /// ウィンドウプロシージャをフックします。
        /// </summary>
        /// <param name="window">フックするウィンドウ</param>
        private void hookWndProc(Window window)
        {
            // ウィンドプロシージャのフック
            this.handle = new WindowInteropHelper(window).Handle;
            HwndSource source = HwndSource.FromHwnd(this.handle);
            source.AddHook(new HwndSourceHook(WndProc));

            // ホットキーの生成
            hotkeies.Add(HotkeyId.ScreenWhole, new Hotkey(this.handle, HotkeyId.ScreenWhole, CaptureService.Current.CaptureScreenWhole).AddTo(this));
            hotkeies.Add(HotkeyId.ActiveWindow, new Hotkey(this.handle, HotkeyId.ActiveWindow, CaptureService.Current.CaptureActiveWindow).AddTo(this));
            hotkeies.Add(HotkeyId.SelectControl, new Hotkey(this.handle, HotkeyId.SelectControl, CaptureService.Current.CaptureSelectControl).AddTo(this));
            hotkeies.Add(HotkeyId.PageWhole, new Hotkey(this.handle, HotkeyId.PageWhole, CaptureService.Current.CapturePageWhole).AddTo(this));
        }

        /// <summary>
        /// ウィンドウプロシージャ
        /// </summary>
        /// <param name="hWnd">ウィンドウのハンドル</param>
        /// <param name="msg">メッセージ</param>
        /// <param name="wParam">メッセージに関する追加情報</param>
        /// <param name="lParam">メッセージに関する追加情報</param>
        /// <param name="handled">ハンドルフラグ</param>
        /// <returns></returns>
        private static IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == (int)WM.HOTKEY)
            {
                int hotkeyId = (int)wParam;
                foreach (Hotkey hotkey in hotkeies.Values)
                {
                    if ((int)hotkey.Id == hotkeyId)
                    {
                        hotkey.Action();
                        handled = true;
                    }
                }

            }
            return IntPtr.Zero;
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
