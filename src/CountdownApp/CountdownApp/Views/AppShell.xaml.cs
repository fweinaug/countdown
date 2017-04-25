using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CountdownApp
{
  public sealed partial class AppShell : Page
  {
    public static AppShell Current;
    public Frame AppFrame => Frame;

    private readonly AppViewModel viewModel;
    private readonly DispatcherTimer updateTimer;

    private string initialCountdownGuid = null;

    public AppShell()
    {
      InitializeComponent();

      Current = this;

      viewModel = new AppViewModel();
      DataContext = viewModel;

      updateTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
      updateTimer.Tick += UpdateTimerTick;
    }

    public void SelectCountdown(string selectedCountdownGuid)
    {
      if (Frame.Content != null)
      {
        while (Frame.CanGoBack)
        {
          Frame.GoBack();
        }

        var page = Frame.Content as MainPage;

        page?.SelectCountdown(selectedCountdownGuid);
      }
      else if (string.IsNullOrEmpty(initialCountdownGuid))
      {
        initialCountdownGuid = selectedCountdownGuid;
      }
    }

    private void UpdateTimerTick(object sender, object e)
    {
      viewModel.UpdateCountdowns();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManagerBackRequested;

      viewModel.Initialize();
      updateTimer.Start();

      if (!string.IsNullOrEmpty(initialCountdownGuid))
        SelectCountdown(initialCountdownGuid);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
      SystemNavigationManager.GetForCurrentView().BackRequested -= SystemNavigationManagerBackRequested;

      updateTimer.Stop();
    }

    private void SystemNavigationManagerBackRequested(object sender, BackRequestedEventArgs e)
    {
      if (AppFrame.CanGoBack && !e.Handled)
      {
        e.Handled = true;

        AppFrame.GoBack();
      }
    }

    private void FrameNavigated(object sender, NavigationEventArgs e)
    {
      SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Frame.CanGoBack
        ? AppViewBackButtonVisibility.Visible
        : AppViewBackButtonVisibility.Collapsed;
    }
  }
}