using StatefulModel.EventListeners;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCap.Util.Mvvm
{
    /// <summary>
    /// プロパティ変更通知を購読する拡張機能を提供します。
    /// </summary>
    public static class PropertyChangedExtensions
    {
        /// <summary>
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> イベントを購読します。
        /// </summary>
        public static IDisposable Subscribe(this INotifyPropertyChanged source, PropertyChangedEventHandler handler)
        {
            return new PropertyChangedEventListener(source, handler);
        }

        /// <summary>
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> イベントを購読します。
        /// </summary>
        /// <param name="source">イベント ソース。</param>
        /// <param name="action">イベント発生時に、<see cref="PropertyChangedEventArgs.PropertyName"/> を受け取って実行されるメソッド。</param>
        public static IDisposable Subscribe(this INotifyPropertyChanged source, Action<string> action)
        {
            return new PropertyChangedEventListener(source, (sender, args) => action(args.PropertyName));
        }

        /// <summary>
        /// 指定したプロパティ名で発生した <see cref="INotifyPropertyChanged.PropertyChanged"/> イベントを購読します。
        /// </summary>
        /// <param name="source">イベント ソース。</param>
        /// <param name="propertyName">イベントを購読するプロパティの名前。</param>
        /// <param name="action">イベント発生時に実行するメソッド。</param>
        /// <param name="immediately">このメソッドの呼び出し時点で <paramref name="action"/> を 1 度実行する場合は true、それ以外の場合は false。既定値は true です。</param>
        public static ListenerWrapper Subscribe(this INotifyPropertyChanged source, string propertyName, Action action, bool immediately = true)
        {
            return new ListenerWrapper(source).Subscribe(propertyName, action, immediately);
        }

        /// <summary>
        /// ひとつのイベント ソースに対して複数のプロパティの変更通知を購読する機能を提供します。
        /// </summary>
        /// <example>
        /// EventSource // INotifyPropertyChanged object
        ///     .Subscribe(nameof(Property1), () => { ... })
        ///     .Subscribe(nameof(Property2), () => { ... })
        ///     .Subscribe(nameof(Property3), () => { ... })
        ///     .AddTo(this);
        /// </example>
        public class ListenerWrapper : IDisposable
        {
            private readonly PropertyChangedEventListener _listener;

            internal ListenerWrapper(INotifyPropertyChanged source)
            {
                this._listener = new PropertyChangedEventListener(source);
            }

            /// <summary>
            /// 指定したプロパティ名で発生した <see cref="INotifyPropertyChanged.PropertyChanged"/> イベントを購読します。
            /// </summary>
            /// <param name="propertyName">イベントを購読するプロパティの名前。</param>
            /// <param name="action">イベント発生時に実行するメソッド。</param>
            /// <param name="immediately">このメソッドの呼び出し時点で <paramref name="action"/> を 1 度実行する場合は true、それ以外の場合は false。既定値は true です。</param>
            public ListenerWrapper Subscribe(string propertyName, Action action, bool immediately = true)
            {
                if (immediately) action();
                this._listener.Add(propertyName, (sender, args) => action());
                return this;
            }

            void IDisposable.Dispose() => this._listener.Dispose();
        }
    }
}
