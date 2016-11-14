using System.IO;
using System.Reflection;
using WinCap.Util.Desktop;

namespace WinCap.Models
{
    /// <summary>
    /// ショートカットの作成機能を提供する基底クラスです。
    /// </summary>
    public abstract class BaseShortcut
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
        public BaseShortcut()
            : this(Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location))
        {
        }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="path">ショートカットのファイル名</param>
        public BaseShortcut(string fileName)
        {
            this.path = getShortcutFilePath(fileName);
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
        /// ショートカットのファイルパスを取得します。
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <returns>ファイルパス</returns>
        protected abstract string getShortcutFilePath(string fileName);
    }
}
