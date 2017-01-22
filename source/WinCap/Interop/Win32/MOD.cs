using System;

namespace WinCap.Interop.Win32
{
    /// <summary>
    /// 修飾キー
    /// </summary>
    [Flags]
    public enum MOD
    {
        /// <summary>
        /// Alt
        /// </summary>
        ALT = 0x0001,

        /// <summary>
        /// Control
        /// </summary>
        CONTROL = 0x0002,

        /// <summary>
        /// Shift
        /// </summary>
        SHIFT = 0x0004,

        /// <summary>
        /// Windows
        /// </summary>
        WIN = 0x0008,

        /// <summary>
        /// NoRepeate
        /// </summary>
        NOREPEAT = 0x4000,
    }
}
