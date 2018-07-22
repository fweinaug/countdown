using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CountdownApp
{
  public sealed partial class SettingsPage : Page
  {
    public SettingsPage()
    {
      InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
      base.OnNavigatedTo(e);

      var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
      coreTitleBar.LayoutMetricsChanged += TitleBarLayoutMetricsChanged;

      var package = Package.Current;
      var version = package.Id.Version;

      AppNameTextBlock.Text = package.DisplayName;
      AppVersionTextBlock.Text = string.Format("Version {0}.{1}.{2}", version.Major, version.Minor, version.Build);
      DevNameTextBlock.Text = package.PublisherDisplayName;

      var theme = Settings.Current.AppTheme.ToString();

      foreach (var uiElement in ThemePanel.Children)
      {
        var radioButton = uiElement as RadioButton;
        if (radioButton != null)
          radioButton.IsChecked = Equals(radioButton.Tag, theme);
      }
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

    private void RadioButtonThemeChecked(object sender, RoutedEventArgs e)
    {
      var radioButton = (RadioButton)sender;

      Settings.Current.AppTheme = Convert.ToInt32(radioButton.Tag);
    }

    private async void ReviewClick(object sender, RoutedEventArgs e)
    {
      var uri = new Uri("ms-windows-store://review/?ProductId=9mt4ppq41wxm");
      await Launcher.LaunchUriAsync(uri);
    }

    private async void FeedbackClick(object sender, RoutedEventArgs e)
    {
      var uri = new Uri("mailto:fweinaug-apps@outlook.com?subject=" + AppNameTextBlock.Text);
      await Launcher.LaunchUriAsync(uri);
    }

    private async void ChangelogClick(object sender, RoutedEventArgs e)
    {
      var dialog = (ContentDialog)FindName("ChangelogContentDialog");
      if (dialog != null)
      {
        await ChangelogView.LoadAndShowChangelog();
        await dialog.ShowAsync();
      }
    }
  }
}