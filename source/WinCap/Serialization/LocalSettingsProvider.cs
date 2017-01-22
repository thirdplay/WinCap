using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;
using WinCap.Util.Serialization;

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
        private readonly FileInfo _targetFile;

        /// <summary>
        /// 利用可能状態
        /// </summary>
        public bool Available { get; }

        /// <summary>
        /// 対象ファイルのファイルパス
        /// </summary>
        public string FilePath => this._targetFile.FullName;

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
                "WinCap",
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

            this._targetFile = file;
            this.Available = true;
        }

        /// <summary>
        /// 非同期保存のコア処理。
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
                    File.Copy(tempFileName, this._targetFile.FullName, true);
                    File.Delete(tempFileName);
                }
            });
        }

        /// <summary>
        /// 非同期読み込みのコア処理。
        /// </summary>
        /// <returns>タスク</returns>
        protected override Task<IDictionary<string, object>> LoadAsyncCore()
        {
            if (!this.Available || !this._targetFile.Exists)
            {
                return Task.FromResult<IDictionary<string, object>>(null);
            }

            return Task.Run(() =>
            {
                if (!this._targetFile.Exists)
                {
                    return null;
                }

                var serializer = new DataContractSerializer(typeof(IDictionary<string, object>), this.KnownTypes);

                try
                {
                    using (var stream = this._targetFile.OpenRead())
                    {
                        return serializer.ReadObject(stream) as IDictionary<string, object>;
                    }
                }
                catch (SerializationException)
                {
#if DEBUG
                    // デバッグ中は逆シリアル化エラーを無視する
                    return null;
#else
                    throw;
#endif
                }
            });
        }
    }
}
