using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WinCap.Serialization
{
    /// <summary>
    /// プロパティのキャッシュ機能を提供します。
    /// </summary>
    public abstract class SettingsHost
    {
        /// <summary>
        /// 静的な設定管理マップ
        /// </summary>
        private static readonly Dictionary<Type, SettingsHost> instances = new Dictionary<Type, SettingsHost>();

        /// <summary>
        /// プロパティのキャッシュ管理用マップ
        /// </summary>
        private readonly Dictionary<string, object> cachedProperties = new Dictionary<string, object>();

        /// <summary>
        /// カテゴリー名
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
        /// プロパティを取得します。
        /// </summary>
        /// <typeparam name="T">SerializablePropertyBaseを継承したクラス</typeparam>
        /// <param name="create">プロパティの生成メソッド</param>
        /// <param name="propertyName">プロパティ名</param>
        /// <returns>プロパティ</returns>
        protected T Cache<T>(Func<string, T> create, [CallerMemberName] string propertyName = "")
        {
            var key = this.CategoryName + "." + propertyName;

            object obj;
            if (this.cachedProperties.TryGetValue(key, out obj) && obj is T)
            {
                return (T)obj;
            }

            var property = create(key);
            this.cachedProperties[key] = property;

            return property;
        }

        /// <summary>
        /// 一意なインスタンスを取得します。
        /// </summary>
        /// <typeparam name="T">任意のクラス</typeparam>
        /// <returns>インスタンス</returns>
        public static T Instance<T>() where T : SettingsHost
        {
            SettingsHost host;
            return instances.TryGetValue(typeof(T), out host) ? (T)host : null;
        }
    }
}
