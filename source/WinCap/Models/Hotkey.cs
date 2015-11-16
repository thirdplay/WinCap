using WinCap.Win32;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WinCap.Models
{
    /// <summary>
    /// ホットキーの取り付け、分離機能。
    /// </summary>
    public class Hotkey : IDisposable
    {
        #region フィールド
        /// <summary>
        /// ウィンドウのハンドル
        /// </summary>
        private IntPtr handle;
        #endregion

        #region プロパティ
        /// <summary>
        /// ホットキーIDを取得します。
        /// </summary>
        public HotkeyId Id { get; }

        /// <summary>
        /// ホットキー押下時に呼ぶアクションを取得します。
        /// </summary>
        public Action Action { get; }

        /// <summary>
        /// ホットキーを取り付けたかどうか返却する。
        /// </summary>
        public bool IsAttached { get; set; }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="handle">ウィンドウのハンドル</param>
        /// <param name="id">ホットキーID</param>
        /// <param name="action">ホットキー押下時に呼ぶアクション</param>
        public Hotkey(IntPtr handle, HotkeyId id, Action action)
        {
            this.handle = handle;
            this.Id = id;
            this.Action = action;
        }

        /// <summary>
        /// 指定された<see cref="key"/>のホットキーを取り付ける。
        /// </summary>
        /// <param name="key">キーコードと修飾子</param>
        /// <returns>成功ならtrue、それ以外はfalseを返却します。</returns>
        public bool Attach(Keys key)
        {
            if (!this.IsAttached)
            {
                if (NativeMethods.RegisterHotKey(handle, (int)this.Id, toModCode(key), (uint)(key & Keys.KeyCode)) != 0)
                {
                    this.IsAttached = true;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// <see cref="Attach(Keys)"/>で取り付けたホットキーを分離する。
        /// </summary>
        public bool Detach()
        {
            if (this.IsAttached)
            {
                if (NativeMethods.UnregisterHotKey(handle, (int)this.Id) != 0)
                {
                    this.IsAttached = false;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// <see cref="key"/>を修飾子コード(Win32)に変換して返却します。
        /// </summary>
        /// <param name="keys">キーコードと修飾子</param>
        /// <returns>修飾子コード</returns>
        private static uint toModCode(Keys key)
        {
            uint code = 0;
            if (((key & Keys.Modifiers) & Keys.Shift) != 0) code |= 0x0004;
            if (((key & Keys.Modifiers) & Keys.Control) != 0) code |= 0x0002;
            if (((key & Keys.Modifiers) & Keys.Alt) != 0) code |= 0x0001;
            return code;
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
