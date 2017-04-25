using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace CountdownApp
{
  public class CountdownDateConverter : IValueConverter
  {
    private static readonly ResourceLoader resourceLoader = ResourceLoader.GetForViewIndependentUse();

    public object Convert(object value, Type targetType, object parameter, string language)
    {
      var date = (DateTime)value;

      var format = resourceLoader.GetString("CountdownDateTimeFormatString");

      return date.ToString(format);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
}