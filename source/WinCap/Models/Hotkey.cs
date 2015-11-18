using WinCap.Win32;
using System;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace WinCap.Models
{
    /// <summary>
    /// ホットキーを取り付け、分離する機能を提供します。
    /// </summary>
    public class Hotkey : IDisposable
    {
        #region const members
        /// <summary>
        /// ホットキーIDの最小値
        /// </summary>
        private const int HotkeyIdMin = 0x0000;

        /// <summary>
        /// ホットキーIDの最大値
        /// </summary>
        private const int HotkeyIdMax = 0xbfff;

        /// <summary>
        /// 修飾キーAlt
        /// </summary>
        private const uint ModAlt = 0x0001;

        /// <summary>
        /// 修飾キーControl
        /// </summary>
        private const uint ModControl = 0x0002;

        /// <summary>
        /// 修飾キーShift
        /// </summary>
        private const uint ModShift = 0x0004;
        #endregion

        #region DllImport
        [DllImport("user32.dll")]
        public static extern int RegisterHotKey(IntPtr hWnd, int id, uint modKey, uint key);

        [DllImport("user32.dll")]
        public static extern int UnregisterHotKey(IntPtr hWnd, int id);
        #endregion

        #region フィールド
        /// <summary>
        /// ウィンドウのハンドル
        /// </summary>
        private IntPtr handle;

        /// <summary>
        /// ホットキー押下時に呼ぶアクション
        /// </summary>
        private Action action;
        #endregion

        #region プロパティ
        /// <summary>
        /// ホットキーIDを取得します。
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// ホットキーを取り付けたかどうか返却する。
        /// </summary>
        public bool IsAttached { get; private set; }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="handle">ウィンドウのハンドル</param>
        /// <param name="id">ホットキーID</param>
        /// <param name="action">ホットキー押下時に呼ぶアクション</param>
        public Hotkey(IntPtr handle, int id, Action action)
        {
            if (handle == IntPtr.Zero) throw new ArgumentException();
            if (id < HotkeyIdMin || id > HotkeyIdMax) throw new ArgumentOutOfRangeException();

            this.handle = handle;
            this.Id = id;
            this.action = action;
        }

        /// <summary>
        /// 指定された<see cref="key"/>のホットキーを取り付けます。
        /// </summary>
        /// <param name="key">キーコードと修飾子</param>
        /// <returns>成功ならtrue、それ以外はfalseを返却します。</returns>
        public bool Attach(Keys key)
        {
            if (this.IsAttached) return false;
            if (RegisterHotKey(handle, this.Id, toModCode(key), (uint)(key & Keys.KeyCode)) != 0)
            {
                HwndSource source = HwndSource.FromHwnd(this.handle);
                source.AddHook(WndProc);
                this.IsAttached = true;
            }
            return this.IsAttached;
        }

        /// <summary>
        /// <see cref="Attach(Keys)"/>で取り付けたホットキーを分離します。
        /// </summary>
        /// <remarks>成功ならtrue、それ以外はfalseを返却します。</remarks>
        public bool Detach()
        {
            if (!this.IsAttached) return false;
            if (UnregisterHotKey(handle, (int)this.Id) != 0)
            {
                HwndSource source = HwndSource.FromHwnd(this.handle);
                source.RemoveHook(WndProc);
                this.IsAttached = false;
            }
            return !this.IsAttached;
        }

        /// <summary>
        /// <see cref="key"/>を修飾子コード(Win32)に変換して返却します。
        /// </summary>
        /// <param name="keys">キーコードと修飾子</param>
        /// <returns>修飾子コード</returns>
        private static uint toModCode(Keys key)
        {
            uint code = 0;
            if (((key & Keys.Modifiers) & Keys.Shift) != 0) code |= ModShift;
            if (((key & Keys.Modifiers) & Keys.Control) != 0) code |= ModControl;
            if (((key & Keys.Modifiers) & Keys.Alt) != 0) code |= ModAlt;
            return code;
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
        private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == (int)WM.HOTKEY && (int)this.Id == (int)wParam)
            {
                this.action();
                handled = true;
            }
            return IntPtr.Zero;
        }

        #region IDisposable members
        /// <summary>
        /// このインスタンスによって使用されているリソースを全て破棄します。
        /// </summary>
        public void Dispose()
        {
            Detach();
        }
        #endregion
    }
}
