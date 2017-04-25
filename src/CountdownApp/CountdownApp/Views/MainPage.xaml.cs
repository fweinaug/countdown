using Windows.UI.Xaml.Controls;

namespace CountdownApp
{
  public sealed partial class MainPage : Page
  {
    public MainPage()
    {
      InitializeComponent();
    }

    public void SelectCountdown(string selectedCountdownGuid)
    {
      MasterDetailsView.SelectCountdown(selectedCountdownGuid);
    }
  }
}