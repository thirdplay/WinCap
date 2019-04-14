using System;
using System.Globalization;
using System.Windows.Data;
using WinCap.Capturers;

namespace WinCap.Views.Converters
{
    /// <summary>
    /// スクリーン座標をコントロール座標に変換して返すコンバーター。
    /// </summary>
    public class ScreenPointConverter : IValueConverter
    {
        /// <summary>
        /// スクリーン座標をコントロール座標に変換して返します。
        /// </summary>
        /// <param name="value">変換対象の値</param>
        /// <param name="targetType">変換対象の型</param>
        /// <param name="parameter">X=X座標,X!=Y座標</param>
        /// <param name="culture">カルチャ</param>
        /// <returns>変換後の値</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double)) throw new ArgumentException(nameof(value));
            var origin = ScreenHelper.GetScreenOrigin();
            return (parameter.ToString() == "X")
                ? (double)value - origin.X
                : (double)value - origin.Y;
        }

        /// <summary>
        /// コントロール座標をスクリーン座標に変換して返します。
        /// </summary>
        /// <param name="value">変換対象の値</param>
        /// <param name="targetType">変換対象の型</param>
        /// <param name="parameter">X=X座標,X!=Y座標</param>
        /// <param name="culture">カルチャ</param>
        /// <returns>変換後の値</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double)) throw new ArgumentException(nameof(value));
            var origin = ScreenHelper.GetScreenOrigin();
            return (parameter.ToString() == "X")
                ? (double)value + origin.X
                : (double)value + origin.Y;
        }
    }
}
