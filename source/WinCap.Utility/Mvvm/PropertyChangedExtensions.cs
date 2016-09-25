using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace WinCap.Utility.Mvvm
{
    public static class PropertyChangedExtensions
    {
        /// <summary>
        /// 指定された<see cref="propertyName"/>の変更イベントを購読します。
        /// </summary>
        /// <param name="source">イベントソース</param>
        /// <param name="propertyName">プロパティ名</param>
        /// <returns>観測シーケンス</returns>
        public static IObservable<PropertyChangedEventArgs> ObserveProperty(this INotifyPropertyChanged source, string propertyName)
        {
            return Observable.FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => (s, e) => h(e),
                h => source.PropertyChanged += h,
                h => source.PropertyChanged -= h)
                .Where(e => e.PropertyName == propertyName);
        }
    }
}
