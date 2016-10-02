using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace WinCap.Util.Serialization
{
    /// <summary>
    /// プロパティのシリアル化機能を提供する基本クラス。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("Value={Value}, Key={Key}, Default={Default}")]
    public abstract class SerializablePropertyBase<T> : INotifyPropertyChanged
    {
        /// <summary>
        /// プロパティ値
        /// </summary>
        private T _value;

        /// <summary>
        /// キャッシュ状態
        /// </summary>
        private bool _cached;

        /// <summary>
        /// キー
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// シリアル化機能を提供するインターフェイス
        /// </summary>
        public ISerializationProvider Provider { get; }

        /// <summary>
        /// 自動保存状態
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
                if (this._cached) { return this._value; }

                if (!this.Provider.IsLoaded)
                {
                    this.Provider.Load();
                }

                object obj;
                if (this.Provider.TryGetValue(this.Key, out obj))
                {
                    this._value = this.DeserializeCore(obj);
                    this._cached = true;
                }
                else
                {
                    this._value = this.Default;
                }

                return this._cached ? this._value : this.Default;
            }
            set
            {
                if (this._cached && Equals(this._value, value)) { return; }

                if (!this.Provider.IsLoaded)
                {
                    this.Provider.Load();
                }

                var old = this._value;
                this._value = value;
                this._cached = true;
                this.Provider.SetValue(this.Key, this.SerializeCore(value));
                this.OnValueChanged(old, value);

                if (this.AutoSave) { this.Provider.Save(); }
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="key"></param>
        /// <param name="provider"></param>
        protected SerializablePropertyBase(string key, ISerializationProvider provider) : this(key, provider, default(T)) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="key"></param>
        /// <param name="provider"></param>
        /// <param name="defaultValue"></param>
        protected SerializablePropertyBase(string key, ISerializationProvider provider, T defaultValue)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            this.Key = key;
            this.Provider = provider;
            this.Default = defaultValue;

            this.Provider.Reloaded += (sender, args) =>
            {
                if (this._cached)
                {
                    this._cached = false;

                    var oldValue = this._value;
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
        /// <param name="listener"></param>
        /// <returns></returns>
        public virtual IDisposable Subscribe(Action<T> listener)
        {
            listener(this.Value);
            return new ValueChangedEventListener(this, listener);
        }

        /// <summary>
        /// リセット
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
                    this._value = default(T);
                    this._cached = false;
                    this.OnValueChanged(this.DeserializeCore(old), this.Default);

                    if (this.AutoSave) this.Provider.Save();
                }
            }
        }

        /// <summary>
        /// 値変更イベントのリスナー。
        /// </summary>
        private class ValueChangedEventListener : IDisposable
        {
            private readonly Action<T> _listener;
            private readonly SerializablePropertyBase<T> _source;

            public ValueChangedEventListener(SerializablePropertyBase<T> property, Action<T> listener)
            {
                this._listener = listener;
                this._source = property;
                this._source.ValueChanged += this.HandleValueChanged;
            }

            private void HandleValueChanged(object sender, ValueChangedEventArgs<T> args)
            {
                this._listener(args.NewValue);
            }

            public void Dispose()
            {
                this._source.ValueChanged -= this.HandleValueChanged;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        public static implicit operator T(SerializablePropertyBase<T> property)
        {
            return property.Value;
        }

        #region events
        /// <summary>
        /// プロパティ値変更イベント
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
        /// 
        /// </summary>
        private readonly Dictionary<PropertyChangedEventHandler, EventHandler<ValueChangedEventArgs<T>>> _handlers
            = new Dictionary<PropertyChangedEventHandler, EventHandler<ValueChangedEventArgs<T>>>();

        /// <summary>
        /// プロパティ変更イベント
        /// </summary>
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { this.ValueChanged += (this._handlers[value] = (sender, args) => value(sender, new PropertyChangedEventArgs(nameof(this.Value)))); }
            remove
            {
                EventHandler<ValueChangedEventArgs<T>> handler;
                if (this._handlers.TryGetValue(value, out handler))
                {
                    this.ValueChanged -= handler;
                    this._handlers.Remove(value);
                }
            }
        }
        #endregion
    }
}
