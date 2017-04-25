using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CountdownApp
{
  public class AppViewModel : ViewModelBase
  {
    private readonly CountdownService countdownService = new CountdownService();
    private readonly NavigationService navigationService = new NavigationService();

    private readonly CountdownCollection countdownCollection = new CountdownCollection();

    private bool countdownsEmpty = true;
    private CountdownViewModel selectedCountdown = null;
    private bool isBusy = false;

    private Command createCountdownCommand = null;
    private Command editCountdownCommand = null;
    private Command deleteCountdownCommand = null;
    private Command pinCountdownCommand = null;
    private Command unpinCountdownCommand = null;
    private Command showSettingsCommand = null;

    private Command previousCountdownCommand = null;
    private Command nextCountdownCommand = null;

    public ObservableCollection<CountdownViewModel> Countdowns
    {
      get { return countdownCollection.Items; }
    }

    public bool CountdownsEmpty
    {
      get { return countdownsEmpty; }
      set { SetProperty(ref countdownsEmpty, value); }
    }

    public CountdownViewModel SelectedCountdown
    {
      get { return selectedCountdown; }
      set { SetProperty(ref selectedCountdown, value); }
    }

    public bool IsBusy
    {
      get { return isBusy; }
      set { SetProperty(ref isBusy, value); }
    }

    public ICommand CreateCountdownCommand
    {
      get { return createCountdownCommand ?? (createCountdownCommand = new Command(CreateCountdown)); }
    }

    public ICommand EditCountdownCommand
    {
      get { return editCountdownCommand ?? (editCountdownCommand = new Command(EditCountdown)); }
    }

    public ICommand DeleteCountdownCommand
    {
      get { return deleteCountdownCommand ?? (deleteCountdownCommand = new Command(DeleteCountdown)); }
    }

    public ICommand PinCountdownCommand
    {
      get { return pinCountdownCommand ?? (pinCountdownCommand = new Command(PinCountdown)); }
    }

    public ICommand UnpinCountdownCommand
    {
      get { return unpinCountdownCommand ?? (unpinCountdownCommand = new Command(UnpinCountdown)); }
    }

    public ICommand PreviousCountdownCommand
    {
      get { return previousCountdownCommand ?? (previousCountdownCommand = new Command(PreviousCountdown)); }
    }

    public ICommand NextCountdownCommand
    {
      get { return nextCountdownCommand ?? (nextCountdownCommand = new Command(NextCountdown)); }
    }

    public ICommand ShowSettingsCommand
    {
      get { return showSettingsCommand ?? (showSettingsCommand = new Command(ShowSettings)); }
    }

    private void CreateCountdown()
    {
      var viewModel = new EditCountdownViewModel(this)
      {
        Date = DateTime.Today
      };

      navigationService.EditCountdown(viewModel);
    }

    private void EditCountdown()
    {
      var countdown = selectedCountdown;

      var viewModel = new EditCountdownViewModel(this)
      {
        CountdownId = countdown.CountdownId,
        Name  = countdown.Name,
        Date = countdown.SelectedDate.Date,
        Time = countdown.SelectedDate.TimeOfDay,
        IsRecurrent = countdown.IsRecurrent,
        ImageData = countdown.ImageData,
        FinishedNotification = countdown.FinishedNotification,
        PinnedToStart = countdown.PinnedToStart
      };

      navigationService.EditCountdown(viewModel);
    }

    private async void DeleteCountdown()
    {
      var countdown = selectedCountdown;

      await countdownService.DeleteCountdown(countdown.CountdownId);

      CountdownViewModel nextCountdown;
      countdownCollection.RemoveCountdown(countdown, out nextCountdown);

      SelectedCountdown = nextCountdown;

      UpdateCountdownsEmpty();
    }

    private async void PinCountdown()
    {
      var countdown = selectedCountdown;

      var success = await countdownService.PinCountdown(countdown.CountdownId);
      if (success)
        countdown.PinnedToStart = true;
    }

    private async void UnpinCountdown()
    {
      var countdown = selectedCountdown;

      var success = await countdownService.UnpinCountdown(countdown.CountdownId);
      if (success)
        countdown.PinnedToStart = false;
    }

    private void PreviousCountdown()
    {
      var countdown = countdownCollection.PreviousCountdown(selectedCountdown);
      if (countdown != null)
        SelectedCountdown = countdown;
    }

    private void NextCountdown()
    {
      var countdown = countdownCollection.NextCountdown(selectedCountdown);
      if (countdown != null)
        SelectedCountdown = countdown;
    }

    private void ShowSettings()
    {
      navigationService.ShowSettings();
    }

    public void Initialize()
    {
      try
      {
        IsBusy = true;

        var countdowns = countdownService.GetCountdowns();
        countdownCollection.Initialize(countdowns);

        UpdateCountdownsEmpty();
      }
      finally
      {
        IsBusy = false;
      }
    }

    public void UpdateCountdowns()
    {
      countdownCollection.UpdateCountdowns();
    }

    public void RefreshCountdown(Countdown countdown)
    {
      SelectedCountdown = countdownCollection.RefreshCountdown(countdown);

      UpdateCountdownsEmpty();
    }

    private void UpdateCountdownsEmpty()
    {
      CountdownsEmpty = countdownCollection.IsEmpty;
    }
  }
}