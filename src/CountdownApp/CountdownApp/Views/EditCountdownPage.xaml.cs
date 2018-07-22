using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CountdownApp
{
  public sealed partial class EditCountdownPage : Page
  {
    public EditCountdownPage()
    {
      InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
      coreTitleBar.LayoutMetricsChanged += TitleBarLayoutMetricsChanged;

      TitleBarLayoutMetricsChanged(coreTitleBar, null);

      DataContext = e.Parameter;
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
      base.OnNavigatedFrom(e);

      var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
      coreTitleBar.LayoutMetricsChanged -= TitleBarLayoutMetricsChanged;
    }

    private void TitleBarLayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
    {
      if (Content is Grid grid)
        grid.Padding = new Thickness(0, sender.Height, 0, 0);
    }
  }
}