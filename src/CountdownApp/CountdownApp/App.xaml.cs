﻿using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Globalization;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Microsoft.EntityFrameworkCore;
using Microsoft.HockeyApp;

namespace CountdownApp
{
  sealed partial class App : Application
  {
    public App()
    {
      HockeyClient.Current.Configure("3de356bd81244f77b1bac4525df5ef7e");

      InitializeComponent();
      Suspending += OnSuspending;

      using (var database = new CountdownContext())
      {
        database.Database.Migrate();
      }
    }

    protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
    {
      BackgroundActivity.Start(args.TaskInstance);
    }

    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
      InitApp(e.TileId != "App" ? e.TileId : null, e.Arguments, e.PrelaunchActivated);
    }

    protected override void OnActivated(IActivatedEventArgs args)
    {
      if (args.Kind == ActivationKind.ToastNotification)
      {
        var toastArgs = (ToastNotificationActivatedEventArgs)args;

        InitApp(toastArgs.Argument, null, false);
      }
    }

    private void InitApp(string countdownGuid, string arguments, bool prelaunchActivated)
    {
      var applicationView = ApplicationView.GetForCurrentView();

      applicationView.SetPreferredMinSize(new Size(320, 320));

      var titleBar = applicationView.TitleBar;
      if (titleBar != null)
      {
        var titleBarColor = (Color)Current.Resources["SystemAccentColor"];

        titleBar.BackgroundColor = titleBarColor;
        titleBar.ButtonBackgroundColor = titleBarColor;
      }

      var shell = Window.Current.Content as AppShell;

      if (shell == null)
      {
        BackgroundActivity.RegisterBackgroundTask();

        shell = new AppShell
        {
          Language = ApplicationLanguages.Languages[0]
        };

        shell.AppFrame.NavigationFailed += OnNavigationFailed;

        Window.Current.Content = shell;
      }

      if (!string.IsNullOrEmpty(countdownGuid))
        shell.SelectCountdown(countdownGuid);

      if (!prelaunchActivated)
      {
        if (shell.AppFrame.Content == null)
        {
          // Navigate to the first page
          shell.AppFrame.Navigate(typeof(MainPage), arguments);
        }

        Window.Current.Activate();
      }
    }

    private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
      throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
    }

    private void OnSuspending(object sender, SuspendingEventArgs e)
    {
      var deferral = e.SuspendingOperation.GetDeferral();
      //TODO: Save application state and stop any background activity
      deferral.Complete();
    }
  }
}