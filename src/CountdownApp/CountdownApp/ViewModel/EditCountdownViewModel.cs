using System;
using System.Windows.Input;

namespace CountdownApp
{
  public class EditCountdownViewModel : ViewModelBase
  {
    private readonly AppViewModel parent;
    private readonly CountdownService countdownService = new CountdownService();
    private readonly NavigationService navigationService = new NavigationService();

    private int countdownId;
    private string name;
    private DateTimeOffset date;
    private TimeSpan time;
    private bool isRecurrent;

    private byte[] imageData;
    private bool imageSelected;

    private bool finishedNotification;
    private bool pinnedToStart;

    private bool expired;

    private Command selectImageCommand = null;
    private Command removeImageCommand = null;

    private Command saveCommand = null;
    private Command cancelCommand = null;

    public int CountdownId
    {
      get { return countdownId; }
      set { SetProperty(ref countdownId, value); }
    }

    public string Name
    {
      get { return name; }
      set { SetProperty(ref name, value); }
    }

    public DateTimeOffset Date
    {
      get { return date; }
      set
      {
        if (SetProperty(ref date, value))
          UpdateExpired();
      }
    }

    public TimeSpan Time
    {
      get { return time; }
      set
      {
        if (SetProperty(ref time, value))
          UpdateExpired();
      }
    }

    public bool IsRecurrent
    {
      get { return isRecurrent; }
      set
      {
        if (SetProperty(ref isRecurrent, value))
          UpdateExpired();
      }
    }

    public byte[] ImageData
    {
      get { return imageData; }
      set
      {
        if (SetProperty(ref imageData, value))
          ImageSelected = imageData != null && imageData.Length > 0;
      }
    }

    public bool ImageSelected
    {
      get { return imageSelected; }
      private set { SetProperty(ref imageSelected, value); }
    }

    public bool FinishedNotification
    {
      get { return finishedNotification; }
      set { SetProperty(ref finishedNotification, value); }
    }

    public bool PinnedToStart
    {
      get { return pinnedToStart; }
      set { SetProperty(ref pinnedToStart, value); }
    }

    public bool Expired
    {
      get { return expired; }
      private set { SetProperty(ref expired, value); }
    }

    public ICommand SelectImageCommand
    {
      get { return selectImageCommand ?? (selectImageCommand = new Command(SelectImage)); }
    }

    public ICommand RemoveImageCommand
    {
      get { return removeImageCommand ?? (removeImageCommand = new Command(RemoveImage)); }
    }

    public ICommand SaveCommand
    {
      get { return saveCommand ?? (saveCommand = new Command(Save)); }
    }

    public ICommand CancelCommand
    {
      get { return cancelCommand ?? (cancelCommand = new Command(Cancel)); }
    }

    public EditCountdownViewModel(AppViewModel parent)
    {
      this.parent = parent;
    }

    public void ChangeImage(byte[] newImageData)
    {
      ImageData = newImageData;
    }

    private void SelectImage()
    {
      navigationService.SelectImage(this);
    }

    private void RemoveImage()
    {
      if (ImageSelected)
      ImageData = new byte[0];
    }

    private async void Save()
    {
      var countdown = await countdownService.SaveCountdown(countdownId, name, (date + time).DateTime, isRecurrent,
        imageData, !expired && finishedNotification, pinnedToStart);

      parent.RefreshCountdown(countdown);
      navigationService.GoBack();
    }

    private void Cancel()
    {
      navigationService.GoBack();
    }

    private void UpdateExpired()
    {
      Expired = !(isRecurrent || date + time > DateTime.Now);
    }
  }
}