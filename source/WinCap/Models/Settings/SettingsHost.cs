using MetroTrilithon.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using WinCap.Properties;

namespace WinCap.Models.Settings
{
    /// <summary>
    /// 設定のセーブ/ロード/キャッシュ機能を提供します。
    /// </summary>
    public abstract class SettingsHost
    {
        #region フィールド
        /// <summary>
        /// インスタンス
        /// </summary>
        private static readonly Dictionary<Type, SettingsHost> instances = new Dictionary<Type, SettingsHost>();

        /// <summary>
        /// キャッシュプロパティ辞書
        /// </summary>
        private readonly Dictionary<string, object> cachedProperties = new Dictionary<string, object>();
        #endregion

        /// <summary>
        /// カテゴリ名
        /// </summary>
        protected virtual string CategoryName => this.GetType().Name;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected SettingsHost()
        {
            instances[this.GetType()] = this;
        }

        /// <summary>
        /// 現在のインスタンスにキャッシュされている <see cref="SerializableProperty{T}"/>
        /// を取得します。 キャッシュがない場合は <see cref="create"/> に従って生成します。
        /// </summary>
        /// <returns></returns>
        protected SerializableProperty<T> Cache<T>(Func<string, SerializableProperty<T>> create, [CallerMemberName] string propertyName = "")
        {
            var key = this.CategoryName + "." + propertyName;

            object obj;
            if (this.cachedProperties.TryGetValue(key, out obj) && obj is SerializableProperty<T>) return (SerializableProperty<T>)obj;

            var property = create(key);
            this.cachedProperties[key] = property;

            return property;
        }

        /// <summary>
        /// 設定ファイルをメモリ上にロードします。
        /// </summary>
        public static void Load()
        {
            try
            {
                Providers.Local.Load();
            }
            catch (Exception)
            {
                File.Delete(Providers.LocalFilePath);
                Providers.Local.Load();
            }
        }

        /// <summary>
        /// メモリ上の設定をファイルにセーブします。
        /// </summary>
        public static void Save()
        {
            try
            {
                Providers.Local.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Resources.Error_SaveSettingFile, Providers.LocalFilePath, ex.Message),
                    Resources.Error_ErrorCaption, MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        /// <summary>
        /// <typeparamref name="T"/> 型の設定オブジェクトの唯一のインスタンスを取得します。
        /// </summary>
        public static T Instance<T>() where T : SettingsHost, new()
        {
            SettingsHost host;
            return instances.TryGetValue(typeof(T), out host) ? (T)host : new T();
        }
    }
}
