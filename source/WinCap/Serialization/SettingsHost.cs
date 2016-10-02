using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using WinCap.Util.Serialization;

namespace WinCap.Serialization
{
    /// <summary>
    /// シリアル化プロパティを管理します。
    /// </summary>
    public abstract class SettingsHost
    {
        /// <summary>
        /// 静的な設定管理マップ
        /// </summary>
        private static readonly Dictionary<Type, SettingsHost> _instances = new Dictionary<Type, SettingsHost>();

        /// <summary>
        /// プロパティのキャッシュ管理用マップ
        /// </summary>
        private readonly Dictionary<string, object> _cachedProperties = new Dictionary<string, object>();

        /// <summary>
        /// カテゴリー名
        /// </summary>
        protected virtual string CategoryName => this.GetType().Name;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected SettingsHost()
        {
            _instances[this.GetType()] = this;
        }

        /// <summary>
        /// シリアル化プロパティを取得します。
        /// </summary>
        /// <typeparam name="T">SerializablePropertyBaseを継承したクラス</typeparam>
        /// <param name="create">シリアル化プロパティの生成メソッド</param>
        /// <param name="propertyName">プロパティ名</param>
        /// <returns>シリアル化プロパティ</returns>
        protected T Cache<T>(Func<string, T> create, [CallerMemberName] string propertyName = "")
        {
            var key = this.CategoryName + "." + propertyName;

            object obj;
            if (this._cachedProperties.TryGetValue(key, out obj) && obj is T)
            {
                return (T)obj;
            }

            var property = create(key);
            this._cachedProperties[key] = property;

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
            return _instances.TryGetValue(typeof(T), out host) ? (T)host : null;
        }
    }
}
