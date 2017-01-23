using System;
using System.IO;

namespace WinCap.Models
{
    /// <summary>
    /// スタートアップショートカットの作成機能を提供します。
    /// </summary>
    public class StartupShortcut : BaseShortcut
    {
        /// <summary>
        /// ショートカットのファイルパスを取得します。
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <returns>ファイルパス</returns>
        protected override string GetShortcutFilePath(string fileName)
        {
            var dir = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            return Path.Combine(dir, fileName + ".lnk");
        }
    }
}
