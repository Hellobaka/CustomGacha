using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using me.cqp.luohuaming.CustomGacha.UI.ViewModel;

namespace me.cqp.luohuaming.CustomGacha.UI.Converter
{
    [ValueConversion(typeof(string), typeof(string))]
    public class RelativePath2FullPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path =Path.Combine(GachaItemQueryDialogViewModel.RelateivePath, value.ToString());
            if (File.Exists(path))
                return path;
            else
                return @"D:\解包\FGO取\角色\DownloadFaceAtlas1\30.png";
            //TODO: 发布时请替换为默认图片
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
