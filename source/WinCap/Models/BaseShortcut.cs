﻿using System.IO;
using System.Reflection;
using WpfUtility.Desktop;

namespace WinCap.Models
{
    /// <summary>
    /// ショートカットの作成機能を提供する基底クラスです。
    /// </summary>
    public abstract class BaseShortcut
    {
        /// <summary>
        /// パス。
        /// </summary>
        private readonly string path;

        /// <summary>
        /// ショートカットが存在するかどうか確認します。
        /// </summary>
        public bool IsExists => File.Exists(this.path);

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
            this.path = GetShortcutFilePath(fileName);
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
        /// ショートカットを再作成します。
        /// </summary>
        /// <param name="isCreate">作成可否状態</param>
        public void Recreate(bool isCreate)
        {
            if (isCreate)
            {
                this.Create();
            }
            else
            {
                this.Remove();
            }
        }

        /// <summary>
        /// ショートカットのファイルパスを取得します。
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <returns>ファイルパス</returns>
        protected abstract string GetShortcutFilePath(string fileName);
    }
}
