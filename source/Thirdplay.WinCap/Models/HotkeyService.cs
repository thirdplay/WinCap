using Livet;
using MetroTrilithon.Lifetime;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;

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
        /// ウィンドウプロシージャをフックするウィンドウ
        /// </summary>
        private Window window;

        /// <summary>
        /// ウィンドウのハンドル
        /// </summary>
        private IntPtr handle = IntPtr.Zero;
        #endregion

        #region プロパティ
        /// <summary>
        /// 現在のサービスを取得する。
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
            //window.IsLoaded
            this.window = window;
            this.window.Loaded += (sender, e) => Attach();
            //add
            //clear
        }

        /// <summary>
        /// ホットキーを付加する。
        /// </summary>
        private void Attach()
        {
            // ウィンドウプロシージャをフックする
            this.handle = new WindowInteropHelper(window).Handle;
            HwndSource source = HwndSource.FromHwnd(this.handle);
            source.AddHook(new HwndSourceHook(WndProc));

            //HotkeyId[] hotkeys = (HotkeyId[])Enum.GetValues(typeof(HotkeyId));
            //foreach (HotkeyId hotkeyId in hotkeys)
            //{
            //    // ホットキー情報の取得
            //    HotkeyInfo hotkeyInfo = setting.GetHotkeyInfo(hotkeyId);

            //    // キーコードがNoneの場合は処理しない
            //    if (hotkeyInfo.Keys == Keys.None)
            //    {
            //        continue;
            //    }

            //    // ホットキーの登録
            //    if (NativeMethods.RegisterHotKey(Handle, (int)hotkeyId, (uint)hotkeyInfo.GetModifiers(), (uint)hotkeyInfo.Keys) == 0)
            //    {
            //        MessageBoxExt.ShowWarning("ホットキーの登録に失敗しました。\nホットキーを変更するか、ホットキーを使用している別ソフトを終了して下さい。");
            //        break;
            //    }
            //}
            //NativeMethods.RegisterHotKey(handle, (int)HotkeyId.ScreenWhole, (uint)MOD.NONE, (uint)VK.PRINT);
            //NativeMethods.UnregisterHotKey(handle, (int)HotkeyId.ScreenWhole, (uint)MOD.NONE, (uint)VK.PRINT);
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
            return IntPtr.Zero;
        }

        #region IDisposableHoloder members
        ICollection<IDisposable> IDisposableHolder.CompositeDisposable => this.compositeDisposable;

        /// <summary>
        /// このインスタンスによって使用されているリソースを全て破棄する。
        /// </summary>
        public void Dispose()
        {
            this.compositeDisposable.Dispose();
        }
        #endregion
    }
}
