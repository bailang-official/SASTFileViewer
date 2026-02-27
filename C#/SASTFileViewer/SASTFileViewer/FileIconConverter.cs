using System;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace SASTFileViewer
{
    // 颜色转换器只负责颜色，绝不涉及图标逻辑
    public class FileColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isFolder = (value is bool b) && b;
            return isFolder ? new SolidColorBrush(Colors.Goldenrod) : new SolidColorBrush(Colors.SteelBlue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }
}