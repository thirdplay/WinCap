﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using WinCap.Util.Serialization;

namespace WinCap.Util.Serialization
{
    /// <summary>
    /// 複数の設定をシリアル化する機能を提供します。
    /// </summary>
    public abstract class DictionaryProvider : ISerializationProvider
    {
        /// <summary>
        /// 非同期オブジェクト
        /// </summary>
        private readonly object _sync = new object();

        /// <summary>
        /// 設定の管理マップ
        /// </summary>
        private Dictionary<string, object> _settings = new Dictionary<string, object>();

        /// <summary>
        /// 読み込み状態
        /// </summary>
        public bool IsLoaded { get; private set; }

        /// <summary>
        /// ファイル名
        /// </summary>
        public virtual string Filename { get; } = "Settings.xml";

        /// <summary>
        /// 
        /// </summary>
        public virtual Type[] KnownTypes { get; } = { typeof(bool), typeof(int[]), };

        /// <summary>
        /// 設定のリロードが発生したときに発生します。
        /// </summary>
        public event EventHandler Reloaded;

        /// <summary>
        /// 指定したキーに値を設定します。
        /// </summary>
        /// <typeparam name="T">任意のクラス</typeparam>
        /// <param name="key">キー</param>
        /// <param name="value">値</param>
        public void SetValue<T>(string key, T value)
        {
            lock (this._sync)
            {
                this._settings[key] = value;
            }
        }

        /// <summary>
        /// 指定したキーに関連付けられている値を取得します。
        /// </summary>
        /// <typeparam name="T">任意のクラス</typeparam>
        /// <param name="key">キー</param>
        /// <param name="value">取得した値</param>
        /// <returns>取得結果</returns>
        public bool TryGetValue<T>(string key, out T value)
        {
            lock (this._sync)
            {
                object obj;
                if (this._settings.TryGetValue(key, out obj) && obj is T)
                {
                    value = (T)obj;
                    return true;
                }
            }

            value = default(T);
            return false;
        }

        /// <summary>
        /// 指定したキーに関連付けられている値を削除します。
        /// </summary>
        /// <param name="key">キー</param>
        /// <returns>削除結果</returns>
        public bool RemoveValue(string key)
        {
            lock (this._sync)
            {
                return this._settings.Remove(key);
            }
        }

        /// <summary>
        /// 設定を保存します。
        /// </summary>
        void ISerializationProvider.Save()
        {
            this.SaveAsync().Wait();
        }

        /// <summary>
        /// 非同期で設定を保存します。
        /// </summary>
        /// <returns>タスク</returns>
        public async Task SaveAsync()
        {
            try
            {
                SortedDictionary<string, object> current;

                lock (this._sync)
                {
                    current = new SortedDictionary<string, object>(this._settings);
                }

                await this.SaveAsyncCore(current).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// 非同期保存のコア処理。
        /// </summary>
        /// <param name="dic">メモリ上の設定内容</param>
        /// <returns></returns>
        protected abstract Task SaveAsyncCore(IDictionary<string, object> dic);

        /// <summary>
        /// 設定を読み込みます。
        /// </summary>
        void ISerializationProvider.Load()
        {
            this.LoadAsync().Wait();
        }

        /// <summary>
        /// 非同期で設定を読み込みます。
        /// </summary>
        /// <returns>タスク</returns>
        public async Task LoadAsync()
        {
            try
            {
                var dic = await this.LoadAsyncCore().ConfigureAwait(false);

                lock (this._sync)
                {
                    this._settings = dic != null
                        ? new Dictionary<string, object>(dic)
                        : new Dictionary<string, object>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            this.IsLoaded = true;
        }

        /// <summary>
        /// 非同期読み込みのコア処理。
        /// </summary>
        /// <returns>読み込んだ設定</returns>
        protected abstract Task<IDictionary<string, object>> LoadAsyncCore();

        /// <summary>
        /// リロードイベントを発生させます。
        /// </summary>
        protected void OnReloaded()
        {
            this.Reloaded?.Invoke(this, EventArgs.Empty);
        }
    }
}