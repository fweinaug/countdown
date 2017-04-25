using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;

namespace CountdownApp
{
  public class Settings : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    public const int DefaultAppTheme = 0;

    public static Settings Current { get; private set; }

    private readonly ApplicationDataContainer container;

    public int AppTheme
    {
      get { return GetValue(DefaultAppTheme); }
      set { SetValue(value); }
    }

    public bool ShowThanks
    {
      get { return GetValue(false); }
      set { SetValue(value); }
    }

    public Settings()
    {
      Current = this;

      container = ApplicationData.Current.RoamingSettings;
    }

    private T GetValue<T>(T defaultValue, [CallerMemberName]string name = null)
    {
      return (T)(container.Values[name] ?? defaultValue);
    }

    private void SetValue(object value, [CallerMemberName]string name = null)
    {
      if (container.Values[name] == value)
        return;

      container.Values[name] = value;

      RaisePropertyChanged(name);
    }

    private void RaisePropertyChanged(string name)
    {
      var handler = PropertyChanged;
      if (handler != null)
      {
        var args = new PropertyChangedEventArgs(name);
        handler(this, args);
      }
    }
  }
}