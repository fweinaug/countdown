using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CountdownApp
{
  public sealed partial class CropImagePage : Page
  {
    public CropImagePage()
    {
      InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      DataContext = e.Parameter;
    }
  }
}