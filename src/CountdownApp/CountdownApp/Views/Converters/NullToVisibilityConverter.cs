using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace CountdownApp
{
  public class NullToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, string language)
    {
      var inverted = false;

      if (parameter != null)
      {
        if (!bool.TryParse(parameter.ToString(), out inverted))
          inverted = false;
      }

      var b = value == null;
      if (b == !inverted)
        return Visibility.Visible;

      return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotImplementedException();
    }
  }
}