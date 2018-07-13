using System;
using System.Drawing;
using System.Windows.Interop;
using WinCap.Interop.Win32;

namespace WinCap.Interop
{
    /// <summary>
    /// Windows 8.1 の Per-Monitor DPI 機能を提供します。
    /// </summary>
    public static class PerMonitorDpi
    {
        /// <summary>
        /// Per-Monitor DPI 機能をサポートしているかどうかを示す値を取得します。
        /// </summary>
        /// <returns>
        /// 動作しているオペレーティング システムが Windows 8.1 (NT 6.3)、もしくは Windows 10 (10.0.x) の場合は true、それ以外の場合は false。
        /// </returns>
        public static bool IsSupported
        {
            get
            {
#if DEBUG
                return true;
#else
                var version = Environment.OSVersion.Version;
                return (version.Major == 6 && version.Minor == 3) || version.Major >= 10;
#endif
            }
        }

        /// <summary>
        /// 指定したハンドルのウィンドウが描画されているモニターの DPI 設定値を取得します。
        /// </summary>
        /// <param name="hWnd">DPI を取得する対象の Win32 ウィンドウを示すウィンドウ ハンドル。</param>
        /// <param name="dpiType">DPI の種類。既定値は <see cref="MonitorDpiType.Default"/> (<see cref="MonitorDpiType.EffectiveDpi"/> と同値) です。</param>
        /// <returns><paramref name="hWnd"/> のウィンドウが描画されているモニターの DPI 設定値。サポートされていないシステムの場合は <see cref="Dpi.Default"/>。</returns>
        public static Dpi GetDpi(IntPtr hWnd, MonitorDpiType dpiType = MonitorDpiType.Default)
        {
            var hMonitor = User32.MonitorFromWindow(hWnd, MonitorDefaultTo.MONITOR_DEFAULTTONEAREST);
            return GetDpiForMonitor(hMonitor, dpiType);
        }

        /// <summary>
        /// 指定した長方形領域との交差部分が最も広いモニターの DPI 設定値を取得します。
        /// </summary>
        /// <param name="rect">DPI を取得する対象の長方形領域</param>
        /// <param name="dpiType">DPI の種類。既定値は <see cref="MonitorDpiType.Default"/> (<see cref="MonitorDpiType.EffectiveDpi"/> と同値) です。</param>
        /// <returns><paramref name="hWnd"/> のウィンドウが描画されているモニターの DPI 設定値。サポートされていないシステムの場合は <see cref="Dpi.Default"/>。</returns>
        public static Dpi GetDpi(Rectangle rect, MonitorDpiType dpiType = MonitorDpiType.Default)
        {
            var r = new RECT()
            {
                left = rect.Left,
                top = rect.Top,
                right = rect.Right,
                bottom = rect.Bottom,
            };
            var hMonitor = User32.MonitorFromRect(ref r, MonitorDefaultTo.MONITOR_DEFAULTTONEAREST);
            return GetDpiForMonitor(hMonitor, dpiType);
        }

        /// <summary>
        /// ディスプレイモニターの DPI 設定値を取得します。
        /// </summary>
        /// <param name="hMonitor">ディスプレイモニタの HMONITOR</param>
        /// <param name="dpiType">DPI の種類。既定値は <see cref="MonitorDpiType.Default"/> (<see cref="MonitorDpiType.EffectiveDpi"/> と同値) です。</param>
        /// <returns><paramref name="hWnd"/> のウィンドウが描画されているモニターの DPI 設定値。サポートされていないシステムの場合は <see cref="Dpi.Default"/>。</returns>
        private static Dpi GetDpiForMonitor(IntPtr hMonitor, MonitorDpiType dpiType)
        {
            if (!IsSupported) { return Dpi.Default; }

            uint dpiX = 1, dpiY = 1;
            SHCore.GetDpiForMonitor(hMonitor, dpiType, out dpiX, out dpiY);

            return new Dpi(dpiX, dpiY);
        }
    }
}
