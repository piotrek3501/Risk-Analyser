using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Risk_analyser.Converters
{
    public class EnumDescriptionConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum enumValue)
            {
                FieldInfo field = enumValue.GetType().GetField(enumValue.ToString());
                DescriptionAttribute attribute = (DescriptionAttribute)field
                    .GetCustomAttributes(typeof(DescriptionAttribute), false)
                    .FirstOrDefault();
                return " "+attribute?.Description ?? " "+enumValue.ToString();
            }
            return " "+value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
