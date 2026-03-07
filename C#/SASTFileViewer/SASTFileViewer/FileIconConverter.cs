using System;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace SASTFileViewer
{
    // 颜色转换器只负责颜色，绝不涉及图标逻辑
    public class FileColorConverter : IValueConverter //实现 IValueConverter 接口，关联XAML中的 Converter 属性
    {
        public object Convert(object value/*IsFolder*/, Type targetType, object parameter, string language)
        {
            bool isFolder = (value is bool b) && b; //检查输入值是否为布尔类型且true，并赋值给isFolder变量
            return isFolder ? new SolidColorBrush(Colors.Goldenrod) : new SolidColorBrush(Colors.SteelBlue); //只有SolidColorBrush类型才能直接在XAML中使用，返回对应颜色的画刷对象
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException(); //关联IValueConverter接口，颜色变数据
    }
}