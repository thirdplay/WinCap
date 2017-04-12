using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;
using WinCap.Properties;
using WpfUtility.Serialization;

namespace WinCap.Serialization
{
    /// <summary>
    /// ローカル設定機能を提供します。
    /// </summary>
    public sealed class LocalSettingsProvider : DictionaryProvider
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        public static LocalSettingsProvider Instance { get; } = new LocalSettingsProvider();

        /// <summary>
        /// 対象ファイル情報
        /// </summary>
        private readonly FileInfo targetFile;

        /// <summary>
        /// 利用可能状態
        /// </summary>
        public bool Available { get; }

        /// <summary>
        /// 対象ファイルのファイルパス
        /// </summary>
        public string FilePath => this.targetFile.FullName;

        /// <summary>
        /// シリアライズ化時に渡す既知の型
        /// </summary>
        public override Type[] KnownTypes { get; } = { typeof(bool) };

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private LocalSettingsProvider()
        {
            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "thirdplay",
                ProductInfo.Title,
                this.FileName);

            var file = new FileInfo(path);
            if (file.Directory == null || file.DirectoryName == null)
            {
                this.Available = false;
                return;
            }

            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }

            this.targetFile = file;
            this.Available = true;
        }

        /// <summary>
        /// 保存の非同期コア処理。
        /// </summary>
        /// <param name="dic">メモリ上の設定内容</param>
        /// <returns>タスク</returns>
        protected override Task SaveAsyncCore(IDictionary<string, object> dic)
        {
            if (!this.Available)
            {
                return Task.FromResult<IDictionary<string, object>>(null);
            }

            return Task.Run(() =>
            {
                var tempFileName = Path.GetTempFileName();
                try
                {
                    var serializer = new DataContractSerializer(dic.GetType(), this.KnownTypes);
                    var settings = new XmlWriterSettings
                    {
                        Indent = true, // more readable!!!
                    };
                    using (var stream = new FileInfo(tempFileName).OpenWrite())
                    using (var writer = XmlWriter.Create(stream, settings))
                    {
                        serializer.WriteObject(writer, dic);
                    }
                }
                finally
                {
                    // 最後に一時ファイルを設定ファイルに上書きする
                    File.Copy(tempFileName, this.targetFile.FullName, true);
                    File.Delete(tempFileName);
                }
            });
        }

        /// <summary>
        /// 読み込みの非同期コア処理。
        /// </summary>
        /// <returns>タスク</returns>
        protected override Task<IDictionary<string, object>> LoadAsyncCore()
        {
            if (!this.Available || !this.targetFile.Exists)
            {
                return Task.FromResult<IDictionary<string, object>>(null);
            }

            return Task.Run(() =>
            {
                if (!this.targetFile.Exists)
                {
                    return null;
                }

                var serializer = new DataContractSerializer(typeof(IDictionary<string, object>), this.KnownTypes);

                using (var stream = this.targetFile.OpenRead())
                {
                    return serializer.ReadObject(stream) as IDictionary<string, object>;
                }
            });
        }
    }
}
