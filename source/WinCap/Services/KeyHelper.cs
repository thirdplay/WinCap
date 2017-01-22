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
            switch (key)
            {
                case Keys.LMenu:
                case Keys.LControlKey:
                case Keys.LShiftKey:
                case Keys.LWin:
                case Keys.RMenu:
                case Keys.RControlKey:
                case Keys.RShiftKey:
                case Keys.RWin:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// 修飾キーかどうか判定します。
        /// </summary>
        /// <param name="key">キーコード</param>
        /// <returns>修飾キーの場合はtrue、それ以外はfalse</returns>
        public static bool IsModifyKey(this Key key)
        {
            return IsModifyKey(key.ToVirtualKey());
        }

        /// <summary>
        /// 修飾キーセットを取得します。
        /// </summary>
        /// <param name="key">仮想キー</param>
        /// <returns>修飾キーセット</returns>
        public static ModifierKeys GetModifierKeys(this Keys key)
        {
            var result = ModifierKeys.None;
            switch (key)
            {
                case Keys.LMenu:
                case Keys.RMenu:
                    result |= ModifierKeys.Alt;
                    break;
                case Keys.LControlKey:
                case Keys.RControlKey:
                    result |= ModifierKeys.Control;
                    break;
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                    result |= ModifierKeys.Shift;
                    break;
                case Keys.LWin:
                case Keys.RWin:
                    result |= ModifierKeys.Windows;
                    break;
            }
            return result;
        }

        /// <summary>
        /// 修飾キーセットを取得します。
        /// </summary>
        /// <param name="key">キーコード</param>
        /// <returns>修飾キーセット</returns>
        public static ModifierKeys GetModifierKeys(this Key key)
        {
            return GetModifierKeys(key.ToVirtualKey());
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
    }
}
