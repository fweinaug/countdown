using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace CountdownApp
{
  public class CountdownDaysConverter : IValueConverter
  {
    private static readonly ResourceLoader resourceLoader = ResourceLoader.GetForViewIndependentUse();

    public object Convert(object value, Type targetType, object parameter, string language)
    {
      var days = (int)value;

      if (days == 0)
        return resourceLoader.GetString("CountdownDaysZeroString");

      var negative = days < 0;
      if (negative)
        days = Math.Abs(days);

      var resource = days != 1 ? "CountdownDaysMultipleFormatString" : "CountdownDaysOneFormatString";
      var format = resourceLoader.GetString(resource);

      return string.Format(format, (negative ? "+" : "") + days);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
}