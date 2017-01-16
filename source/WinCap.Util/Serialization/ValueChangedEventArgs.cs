using System;

namespace WinCap.Util.Serialization
{
    /// <summary>
    /// 値変更イベントの引数
    /// </summary>
    /// <typeparam name="T">任意のクラス</typeparam>
    public class ValueChangedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// 変更前の値
        /// </summary>
        public T OldValue { get; }

        /// <summary>
        /// 変更後の値
        /// </summary>
        public T NewValue { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="oldValue">変更前の値</param>
        /// <param name="newValue">変更後の値</param>
        public ValueChangedEventArgs(T oldValue, T newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }
    }
}
