using System;
using System.Globalization;
using System.Windows.Data;

namespace WinCap.Util.UI.Converters
{
    /// <summary>
    /// 列挙型をboolに変換して返すコンバーターを定義します。
    /// </summary>
    public class EnumToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// 列挙型をboolに変換して返します。
        /// </summary>
        /// <param name="value">変換対象の値</param>
        /// <param name="targetType">変換対象の型</param>
        /// <param name="parameter">列挙定数の名前</param>
        /// <param name="culture">カルチャ</param>
        /// <returns>変換後の値</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() == parameter.ToString();
        }

        /// <summary>
        /// boolを列挙型に変換して返します。
        /// </summary>
        /// <param name="value">bool値</param>
        /// <param name="targetType">変換対象の型</param>
        /// <param name="parameter">列挙定数の名前</param>
        /// <param name="culture">カルチャ</param>
        /// <returns>変換後の値</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Enum.Parse(targetType, parameter.ToString(), true) : null;
        }
    }
}
