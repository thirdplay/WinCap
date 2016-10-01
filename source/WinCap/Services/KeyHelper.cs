using System.Windows.Forms;
using System.Windows.Input;

namespace WinCap.Services
{
    public static class KeyHelper
    {
        public static bool IsModifyKey(this Keys key)
        {
            return IsModifyKey((uint)key);
        }

        public static bool IsModifyKey(this Key key)
        {
            return IsModifyKey((uint)key.ToVirtualKey());
        }

        public static Keys ToVirtualKey(this Key key)
        {
            return (Keys)KeyInterop.VirtualKeyFromKey(key);
        }

        /// <summary>
        /// 装飾キーかどうか判定します。
        /// </summary>
        /// <param name="keyCode">キーコード</param>
        /// <returns>装飾キーの場合はtrue、それ以外はfalse</returns>
        private static bool IsModifyKey(uint keyCode)
        {
            switch (keyCode)
            {
                case 164: // LMenu
                case 162: // LControlKey
                case 160: // LShiftKey
                case 091: // LWin
                case 165: // RMenu
                case 163: // RControlKey
                case 161: // RShiftKey
                case 092: // RWin
                    return true;

                default:
                    return false;
            }
        }
    }
}
