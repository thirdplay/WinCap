using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCap.Win32
{
    /// <summary>
    /// 装飾キー
    /// </summary>
    public enum MOD : uint
    {
        NONE = 0x0000,
        ALT = 0x0001,
        CONTROL = 0x0002,
        SHIFT = 0x0004,
        WIN = 0x0008,
        NOREPEAT = 0x4000,
    }
}
