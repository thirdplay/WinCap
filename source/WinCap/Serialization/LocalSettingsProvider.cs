using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;
using WinCap.Util.Serialization;

namespace WinCap.Serialization
{
    /// <summary>
    /// ローカル設定機能を提供します。
    /// </summary>
    public sealed class LocalSettingsProvider : DictionaryProvider, IDisposable
    {
        public static TimeSpan FileSystemHandlerThrottleDueTime { get; set; } = TimeSpan.FromMilliseconds(1500);

        /// <summary>
        /// インスタンス
        /// </summary>
        public static LocalSettingsProvider Instance { get; } = new LocalSettingsProvider();

        /// <summary>
        /// ファイルシステム監視
        /// </summary>
        private readonly FileSystemWatcher _watcher;

        /// <summary>
        /// 対象ファイル情報
        /// </summary>
        private readonly FileInfo _targetFile;

        /// <summary>
        /// ファイル変更通知リスト
        /// </summary>
        private readonly Subject<FileSystemEventArgs> _notifier;

        /// <summary>
        /// 利用可能状態
        /// </summary>
        public bool Available { get; }

        /// <summary>
        /// 対象ファイルのファイルパス
        /// </summary>
        public string FilePath => this._targetFile.FullName;

        /// <summary>
        /// ファイル変更通知プロバイダ
        /// </summary>
        public IObservable<WatcherChangeTypes> FileChanged => this._notifier.Select(x => x.ChangeType);

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private LocalSettingsProvider()
        {
            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "thirdplay",
                "WinCap",
                this.Filename);

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

            try
            {
                this._notifier = new Subject<FileSystemEventArgs>();
                this._notifier.Throttle(FileSystemHandlerThrottleDueTime)
                    .SelectMany(_ => this.LoadAsync().ToObservable())
                    .Subscribe(_ => this.OnReloaded());

                this._targetFile = file;
                this._watcher = new FileSystemWatcher(file.DirectoryName, file.Name)
                {
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite,
                };
                this._watcher.Changed += this.HandleFileChanged;
                this._watcher.Created += this.HandleFileChanged;
                this._watcher.Deleted += this.HandleFileChanged;
                this._watcher.Renamed += this.HandleFileChanged;
                this._watcher.EnableRaisingEvents = true;

                this.Available = true;
            }
            catch (Exception)
            {
                this.Available = false;
            }
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
                var serializer = new DataContractSerializer(dic.GetType(), this.KnownTypes);
                var settings = new XmlWriterSettings
                {
                    Indent = true, // more readable!!!
                };
                using (var stream = this._targetFile.OpenWrite())
                using (var writer = XmlWriter.Create(stream, settings))
                {
                    serializer.WriteObject(writer, dic);
                }
            });
        }

        /// <summary>
        /// 非同期読み込みのコア処理。
        /// </summary>
        /// <returns>読み込んだ設定</returns>
        protected override Task<IDictionary<string, object>> LoadAsyncCore()
        {
            if (!this.Available || !this._targetFile.Exists)
            {
                return Task.FromResult<IDictionary<string, object>>(null);
            }

            return Task.Run(() =>
            {
                if (!this._targetFile.Exists) return null;

                var serializer = new DataContractSerializer(typeof(IDictionary<string, object>), this.KnownTypes);

                using (var stream = this._targetFile.OpenRead())
                {
                    return serializer.ReadObject(stream) as IDictionary<string, object>;
                }
            });
        }

        /// <summary>
        /// ファイル変更イベント。
        /// </summary>
        /// <param name="sender">イベント発生元オブジェクト</param>
        /// <param name="args">イベント引数</param>
        private void HandleFileChanged(object sender, FileSystemEventArgs args)
        {
            this._notifier.OnNext(args);
        }

        /// <summary>
        /// このインスタンスによって使用されているリソースを全て破棄します。
        /// </summary>
        public void Dispose()
        {
            if (this._watcher != null)
            {
                this._watcher.EnableRaisingEvents = false;
                this._watcher.Changed -= this.HandleFileChanged;
                this._watcher.Created -= this.HandleFileChanged;
                this._watcher.Deleted -= this.HandleFileChanged;
                this._watcher.Renamed -= this.HandleFileChanged;
                this._watcher.Dispose();
            }
        }
    }
}
