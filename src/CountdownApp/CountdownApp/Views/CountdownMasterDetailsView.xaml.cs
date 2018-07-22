using System;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace CountdownApp
{
  public sealed partial class CountdownMasterDetailsView : UserControl
  {
    public static readonly DependencyProperty SelectedCountdownProperty = DependencyProperty.Register(
      "SelectedCountdown", typeof(object), typeof(CountdownMasterDetailsView), new PropertyMetadata(null, OnSelectedCountdownChanged));

    public static readonly DependencyProperty SwipeRightCommandProperty = DependencyProperty.Register(
      "SwipeRightCommand", typeof(ICommand), typeof(CountdownMasterDetailsView), new PropertyMetadata(null));

    public static readonly DependencyProperty SwipeLeftCommandProperty = DependencyProperty.Register(
      "SwipeLeftCommand", typeof(ICommand), typeof(CountdownMasterDetailsView), new PropertyMetadata(null));

    public object SelectedCountdown
    {
      get { return GetValue(SelectedCountdownProperty); }
      set { SetValue(SelectedCountdownProperty, value); }
    }

    public ICommand SwipeRightCommand
    {
      get { return (ICommand)GetValue(SwipeRightCommandProperty); }
      set { SetValue(SwipeRightCommandProperty, value); }
    }

    public ICommand SwipeLeftCommand
    {
      get { return (ICommand)GetValue(SwipeLeftCommandProperty); }
      set { SetValue(SwipeLeftCommandProperty, value); }
    }

    private Point manipulationStartPosition;

    public CountdownMasterDetailsView()
    {
      InitializeComponent();
    }

    public void SetTitleBarHeight(double height)
    {
      MasterGrid.Padding = new Thickness(0, height, 0, 0);
      DetailGrid.Padding = new Thickness(0, height, 0, 0);
    }

    public void SelectCountdown(string selectedCountdownGuid)
    {
      foreach (CountdownViewModel countdown in MasterListView.Items)
      {
        if (countdown.Guid == selectedCountdownGuid)
        {
          SelectedCountdown = countdown;

          var state = AdaptiveStates.CurrentState == NarrowState ? SelectedNarrowState : SelectedState;
          VisualStateManager.GoToState(this, state.Name, true);

          break;
        }
      }
    }

    private static void OnSelectedCountdownChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var view = (CountdownMasterDetailsView)d;

      view.SelectCountdown(e.NewValue as CountdownViewModel);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
      SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManagerBackRequested;

      UpdateBackButton();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
      SystemNavigationManager.GetForCurrentView().BackRequested -= SystemNavigationManagerBackRequested;
    }

    private void SystemNavigationManagerBackRequested(object sender, BackRequestedEventArgs e)
    {
      if (AdaptiveStates.CurrentState == NarrowState && SelectedCountdown != null)
      {
        SelectedCountdown = null;

        VisualStateManager.GoToState(this, UnselectedState.Name, true);

        e.Handled = true;
      }
    }

    private void MasterListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (MasterListView.Items == null || MasterListView.Items.Count == 0)
      {
        SelectedCountdown = null;

        VisualStateManager.GoToState(this, UnselectedState.Name, true);
      }
      else if (AdaptiveStates.CurrentState == DefaultState)
      {
        SelectedCountdown = MasterListView.SelectedItem;

        var state = SelectedCountdown != null ? SelectedState : UnselectedState;
        VisualStateManager.GoToState(this, state.Name, true);
      }
    }

    private void MasterListViewItemClick(object sender, ItemClickEventArgs e)
    {
      if (AdaptiveStates.CurrentState == NarrowState)
      {
        SelectedCountdown = e.ClickedItem;

        VisualStateManager.GoToState(this, SelectedNarrowState.Name, true);
      }
    }

    private void MasterListViewRightTapped(object sender, RightTappedRoutedEventArgs e)
    {
      var tappedItem = (e.OriginalSource as FrameworkElement)?.DataContext as CountdownViewModel;
      if (tappedItem == null)
        return;

      var flyout = FlyoutBase.GetAttachedFlyout(MasterListView) as MenuFlyout;
      if (flyout == null)
        return;

      SelectedCountdown = tappedItem;

      if (MasterListView.SelectionMode == ListViewSelectionMode.None)
      {
        MasterListView.SelectionMode = ListViewSelectionMode.Single;
        MasterListView.SelectedItem = tappedItem;
      }

      var position = e.GetPosition(null);
      flyout.ShowAt(null, position);

      var state = AdaptiveStates.CurrentState == NarrowState ? FocusedState.Name : SelectedState.Name;
      VisualStateManager.GoToState(this, state, true);
    }

    private void MenuFlyoutClosing(FlyoutBase sender, FlyoutBaseClosingEventArgs e)
    {
      if (AdaptiveStates.CurrentState == NarrowState)
      {
        MasterListView.SelectionMode = ListViewSelectionMode.None;

        SelectedCountdown = null;

        VisualStateManager.GoToState(this, UnselectedState.Name, true);
      }
      else
      {
        MasterListView.SelectionMode = ListViewSelectionMode.Single;
      }
    }

    private void AdaptiveStatesCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
    {
      var state = SelectedCountdown == null
        ? UnselectedState
        : AdaptiveStates.CurrentState == NarrowState ? SelectedNarrowState : SelectedState;

      VisualStateManager.GoToState(this, state.Name, true);
    }

    private void SelectionStatesCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
    {
      UpdateBackButton();
    }

    private void ContentControlManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
    {
      manipulationStartPosition = e.Position;
    }

    private void ContentControlManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
      if (e.PointerDeviceType != PointerDeviceType.Touch)
        return;

      var diffY = Math.Abs(e.Position.Y - manipulationStartPosition.Y);
      if (diffY > 100)
        return;

      var diffX = e.Position.X - manipulationStartPosition.X;
      if (diffX > 100)
      {
        SwipeLeftCommand?.Execute(null);
      }
      else if (diffX < 100)
      {
        SwipeRightCommand?.Execute(null);
      }
    }

    private void UpdateBackButton()
    {
      SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = SelectionStates.CurrentState == SelectedNarrowState
        ? AppViewBackButtonVisibility.Visible
        : AppViewBackButtonVisibility.Collapsed;
    }

    private void SelectCountdown(CountdownViewModel countdown)
    {
      MasterListView.SelectedItem = countdown;
    }
  }
}