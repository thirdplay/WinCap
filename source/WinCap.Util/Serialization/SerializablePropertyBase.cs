using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace WinCap.Util.Serialization
{
    /// <summary>
    /// プロパティのシリアル化機能を提供する基本クラス。
    /// </summary>
    /// <typeparam name="T">任意のクラス</typeparam>
    [DebuggerDisplay("Value={Value}, Key={Key}, Default={Default}")]
    public abstract class SerializablePropertyBase<T> : ISerializableProperty, INotifyPropertyChanged
    {
        /// <summary>
        /// プロパティ値
        /// </summary>
        private T value;

        /// <summary>
        /// キャッシュ状態
        /// </summary>
        private bool cached;

        /// <summary>
        /// プロパティを表すキーを取得します。
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// シリアル化機能を提供するインターフェイス
        /// </summary>
        public ISerializationProvider Provider { get; }

        /// <summary>
        /// 自動保存状態を示す値を取得または設定します。
        /// </summary>
        public bool AutoSave { get; set; }

        /// <summary>
        /// デフォルト値
        /// </summary>
        public T Default { get; }

        /// <summary>
        /// プロパティ値
        /// </summary>
        public virtual T Value
        {
            get
            {
                if (this.cached) { return this.value; }

                if (!this.Provider.IsLoaded)
                {
                    this.Provider.Load();
                }

                object obj;
                if (this.Provider.TryGetValue(this.Key, out obj))
                {
                    this.value = this.DeserializeCore(obj);
                    this.cached = true;
                }
                else
                {
                    this.value = this.Default;
                }

                return this.cached ? this.value : this.Default;
            }
            set
            {
                if (this.cached && Equals(this.value, value)) { return; }

                if (!this.Provider.IsLoaded)
                {
                    this.Provider.Load();
                }

                var old = this.value;
                this.value = value;
                this.cached = true;
                this.Provider.SetValue(this.Key, this.SerializeCore(value));
                this.OnValueChanged(old, value);

                if (this.AutoSave) { this.Provider.Save(); }
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="key">プロパティのキー</param>
        /// <param name="provider">シリアル化機能の提供者</param>
        protected SerializablePropertyBase(string key, ISerializationProvider provider) : this(key, provider, default(T)) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="key">プロパティのキー</param>
        /// <param name="provider">シリアル化機能の提供者</param>
        /// <param name="defaultValue">プロパティのデフォルト値</param>
        protected SerializablePropertyBase(string key, ISerializationProvider provider, T defaultValue)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            this.Key = key;
            this.Provider = provider;
            this.Default = defaultValue;

            this.Provider.Reseted += (sender, args) => this.Reset();
            this.Provider.Reloaded += (sender, args) =>
            {
                if (this.cached)
                {
                    this.cached = false;

                    var oldValue = this.value;
                    var newValue = this.Value;
                    if (!Equals(oldValue, newValue))
                    {
                        this.OnValueChanged(oldValue, newValue);
                    }
                }
                else
                {
                    var oldValue = default(T);
                    var newValue = this.Value;
                    this.OnValueChanged(oldValue, newValue);
                }
            };
        }

        /// <summary>
        /// シリアライズ化のコア処理。
        /// </summary>
        /// <param name="value">シリアライズ化前の値</param>
        /// <returns>シリアライズ化後の値</returns>
        protected virtual object SerializeCore(T value)
        {
            return value;
        }

        /// <summary>
        /// デシリアライズ化のコア処理。
        /// </summary>
        /// <param name="value">デシリアライズ化前の値</param>
        /// <returns>デシリアライズ化後の値</returns>
        protected virtual T DeserializeCore(object value)
        {
            return (T)value;
        }

        /// <summary>
        /// プロパティ値の変更を購読します。
        /// </summary>
        /// <param name="listener">リスナーアクション</param>
        /// <returns>購読リクエスト</returns>
        public virtual IDisposable Subscribe(Action<T> listener)
        {
            listener(this.Value);
            return new ValueChangedEventListener(this, listener);
        }

        /// <summary>
        /// プロパティ値をリセットします。
        /// </summary>
        public virtual void Reset()
        {
            if (!this.Provider.IsLoaded)
            {
                this.Provider.Load();
            }

            object old;
            if (this.Provider.TryGetValue(this.Key, out old))
            {
                if (this.Provider.RemoveValue(this.Key))
                {
                    this.value = this.Default;
                    this.cached = false;
                    this.OnValueChanged(this.DeserializeCore(old), this.Default);

                    if (this.AutoSave)
                    {
                        this.Provider.Save();
                    }
                }
            }
        }

        /// <summary>
        /// 値変更イベントのリスナー。
        /// </summary>
        private class ValueChangedEventListener : IDisposable
        {
            private readonly Action<T> listener;
            private readonly SerializablePropertyBase<T> source;

            public ValueChangedEventListener(SerializablePropertyBase<T> property, Action<T> listener)
            {
                this.listener = listener;
                this.source = property;
                this.source.ValueChanged += this.HandleValueChanged;
            }

            private void HandleValueChanged(object sender, ValueChangedEventArgs<T> args)
            {
                this.listener(args.NewValue);
            }

            public void Dispose()
            {
                this.source.ValueChanged -= this.HandleValueChanged;
            }
        }

        /// <summary>
        /// プロパティ値の変換演算子。
        /// </summary>
        /// <param name="property">プロパティ</param>
        public static implicit operator T(SerializablePropertyBase<T> property)
        {
            return property.Value;
        }

        #region events

        /// <summary>
        /// プロパティ値変更イベント。
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<T>> ValueChanged;

        /// <summary>
        /// プロパティ値の変更イベントを発生させる。
        /// </summary>
        /// <param name="oldValue">変更前の値</param>
        /// <param name="newValue">変更後の値</param>
        protected virtual void OnValueChanged(T oldValue, T newValue)
        {
            this.ValueChanged?.Invoke(this, new ValueChangedEventArgs<T>(oldValue, newValue));
        }

        /// <summary>
        /// プロパティ値の変更イベントハンドルのコンテナ。
        /// </summary>
        private readonly Dictionary<PropertyChangedEventHandler, EventHandler<ValueChangedEventArgs<T>>> handlers
            = new Dictionary<PropertyChangedEventHandler, EventHandler<ValueChangedEventArgs<T>>>();

        /// <summary>
        /// プロパティ変更イベント。
        /// </summary>
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { this.ValueChanged += (this.handlers[value] = (sender, args) => value(sender, new PropertyChangedEventArgs(nameof(this.Value)))); }
            remove
            {
                EventHandler<ValueChangedEventArgs<T>> handler;
                if (this.handlers.TryGetValue(value, out handler))
                {
                    this.ValueChanged -= handler;
                    this.handlers.Remove(value);
                }
            }
        }

        #endregion
    }
}
