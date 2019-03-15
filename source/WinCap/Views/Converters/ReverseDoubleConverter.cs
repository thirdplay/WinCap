using System;
using System.Globalization;
using System.Windows.Data;

namespace WinCap.Views.Converters
{
    /// <summary>
    /// double型の値の符号を反転して返すコンバーター。
    /// </summary>
    public class ReverseDoubleConverter : IValueConverter
    {
        /// <summary>
        /// double型の値の符号を反転して返します。
        /// </summary>
        /// <param name="value">変換対象の値</param>
        /// <param name="targetType">変換対象の型</param>
        /// <param name="parameter">コンバートパラメータ</param>
        /// <param name="culture">カルチャ</param>
        /// <returns>変換後の値</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double)) throw new ArgumentException(nameof(value));
            return -(double)value;
        }

        /// <summary>
        /// double型の値の符号を反転して返します。
        /// </summary>
        /// <param name="value">変換対象の値</param>
        /// <param name="targetType">変換対象の型</param>
        /// <param name="parameter">コンバートパラメータ</param>
        /// <param name="culture">カルチャ</param>
        /// <returns>変換後の値</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double)) throw new ArgumentException(nameof(value));
            return -(double)value;
        }
    }
}
