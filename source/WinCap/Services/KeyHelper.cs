using System.Windows.Forms;
using System.Windows.Input;

namespace WinCap.Services
{
    /// <summary>
    /// キーの補助機能を提供します。
    /// </summary>
    public static class KeyHelper
    {
        /// <summary>
        /// 修飾キーかどうか判定します。
        /// </summary>
        /// <param name="key">仮想キー</param>
        /// <returns>修飾キーの場合はtrue、それ以外はfalse</returns>
        public static bool IsModifyKey(this Keys key)
        {
            return IsModifyKey((uint)key);
        }

        /// <summary>
        /// 修飾キーかどうか判定します。
        /// </summary>
        /// <param name="key">キーコード</param>
        /// <returns>修飾キーの場合はtrue、それ以外はfalse</returns>
        public static bool IsModifyKey(this Key key)
        {
            return IsModifyKey((uint)key.ToVirtualKey());
        }

        /// <summary>
        /// 修飾キーセットを取得します。
        /// </summary>
        /// <param name="keys">仮想キー</param>
        /// <returns>修飾キーセット</returns>
        public static ModifierKeys GetModifierKeys(this Keys keys)
        {
            var result = ModifierKeys.None;
            if ((keys & Keys.Alt) == Keys.Alt)
            {
                result |= ModifierKeys.Alt;
            }
            if ((keys & Keys.Control) == Keys.Control)
            {
                result |= ModifierKeys.Control;
            }
            if ((keys & Keys.Shift) == Keys.Shift)
            {
                result |= ModifierKeys.Shift;
            }
            return result;
        }

        /// <summary>
        /// 仮想キーをキーに変換します。
        /// </summary>
        /// <param name="keys">仮想キー</param>
        /// <returns>キー</returns>
        public static Key ToKey(this Keys keys)
        {
            return KeyInterop.KeyFromVirtualKey((int)keys);
        }

        /// <summary>
        /// キーを仮想キーに変換します。
        /// </summary>
        /// <param name="key">キーコード</param>
        /// <returns>仮想キー</returns>
        public static Keys ToVirtualKey(this Key key)
        {
            return (Keys)KeyInterop.VirtualKeyFromKey(key);
        }

        /// <summary>
        /// 修飾キーかどうか判定します。
        /// </summary>
        /// <param name="keyCode">キーコード</param>
        /// <returns>修飾キーの場合はtrue、それ以外はfalse</returns>
        private static bool IsModifyKey(uint keyCode)
        {
            unchecked
            {
                return (keyCode & (uint)Keys.Modifiers) != 0;
            }
        }
    }
}
