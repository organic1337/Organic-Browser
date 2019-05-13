using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Organic_Browser.Converters
{
    /// <summary>
    /// Converts from a path string to Uri object.
    /// 
    /// a path string may be something like '../Resources/image.png' or 
    /// 'pack://application:,,,/Resources/Images/Buttons/Light/right-arrow.png'
    /// </summary>
    class StringToUriConverter : IValueConverter
    {

        public StringToUriConverter()
        {

        }

        /// <summary>
        /// Convert from string to uri
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string stringValue = (string)value;
            return new Uri(stringValue, UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// Convert from uri to string
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Uri uriValue = (Uri)value;
            return uriValue.AbsoluteUri;
        }
    }
}
