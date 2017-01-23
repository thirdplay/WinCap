using System;
using System.IO;

namespace WinCap.Models
{
    /// <summary>
    /// デスクトップショートカットの作成機能を提供します。
    /// </summary>
    public class DesktopShortcut : BaseShortcut
    {
        /// <summary>
        /// ショートカットのファイルパスを取得します。
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <returns>ファイルパス</returns>
        protected override string GetShortcutFilePath(string fileName)
        {
            var dir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            return Path.Combine(dir, fileName + ".lnk");
        }
    }
}
