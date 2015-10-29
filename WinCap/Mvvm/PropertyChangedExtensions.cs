using StatefulModel.EventListeners;
using System;
using System.ComponentModel;

namespace WinCap.Mvvm
{
    /// <summary>
    /// プロパティ変更イベントリスナーの拡張
    /// </summary>
    public static class PropertyChangedExtensions
    {
        /// <summary>
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> イベントを購読する。
        /// </summary>
        /// <param name="source">イベントソース</param>
        /// <param name="propertyName">イベントを購読するプロパティ名</param>
        /// <param name="action">イベント発生時に呼び出すメソッド</param>
        /// <returns>イベントリスナー</returns>
        public static IDisposable Subscribe(this INotifyPropertyChanged source, string propertyName, Action action)
        {
            PropertyChangedEventListener listener = new PropertyChangedEventListener(source);
            listener.Add(propertyName, (sender, args) => action());
            return listener;
        }
    }
}
