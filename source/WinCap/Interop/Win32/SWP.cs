using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCap.Interop.Win32
{
    /// <summary>
    /// ウィンドウのサイズと位置の変更に関するフラグを表します。
    /// </summary>
    [Flags]
    public enum SWP : int
    {
        NOSIZE = 0x0001,
        NOMOVE = 0x0002,
        NOZORDER = 0x0004,
        NOREDRAW = 0x0008,
        NOACTIVATE = 0x0010,

        FRAMECHANGED = 0x0020, // The frame changed: send WM_NCCALCSIZE
        SHOWWINDOW = 0x0040,
        HIDEWINDOW = 0x0080,
        NOCOPYBITS = 0x0100,
        NOOWNERZORDER = 0x0200, // Don’t do owner Z ordering

        NOSENDCHANGING = 0x0400, // Don’t send WM_WINDOWPOSCHANGING
    }
}
