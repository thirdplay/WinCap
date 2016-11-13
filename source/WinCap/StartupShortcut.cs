using System;
using System.IO;
using System.Reflection;
using WinCap.Util.Desktop;

namespace WinCap
{
    /// <summary>
    /// スタートアップショートカットの作成機能を提供します。
    /// </summary>
    public class StartupShortcut
    {
        /// <summary>
        /// パス
        /// </summary>
        private readonly string path;

        /// <summary>
        /// ショートカットが存在するかどうか確認します。
        /// </summary>
        public bool IsExists
        {
            get { return File.Exists(this.path); }
        }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public StartupShortcut()
            : this(Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location))
        {
        }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="path">ショートカットのファイル名</param>
        public StartupShortcut(string fileName)
        {
            this.path = getStartupFilePath(fileName);
        }

        /// <summary>
        /// ショートカットを作成します。
        /// </summary>
        public void Create()
        {
            if (!this.IsExists)
            {
                Shortcut.Create(this.path);
            }
        }

        /// <summary>
        /// ショートカットを削除します。
        /// </summary>
        public void Remove()
        {
            if (this.IsExists)
            {
                File.Delete(this.path);
            }
        }

        /// <summary>
        /// スタートアップショートカットのファイルパスを取得します。
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <returns>ファイルパス</returns>
        private string getStartupFilePath(string fileName)
        {
            var dir = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            return Path.Combine(dir, fileName + ".lnk");
        }
    }
}
