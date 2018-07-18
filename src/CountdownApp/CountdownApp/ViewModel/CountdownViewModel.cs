using System;
using System.Windows.Input;

namespace CountdownApp
{
  public class CountdownViewModel : ViewModelBase
  {
    private Countdown countdown;

    private string name;
    private DateTime selectedDate;
    private bool isRecurrent;
    private bool finishedNotification;
    private bool pinnedToStart;
    private byte[] imageData;

    private DateTime date;
    private Time time;
    private int daysRemaining;
    private int daysTotal;
    private double progressPercentage;
    private DateTime expirationDate;
    private int? repetitionCount;
    private bool expired;

    private bool timeToggled = false;

    private Command toggleTimeCommand = null;

    public int CountdownId
    {
      get { return countdown.Id; }
    }

    public string Guid
    {
      get { return countdown.Guid; }
    }

    public string Name
    {
      get { return name; }
      set { SetProperty(ref name, value); }
    }

    public DateTime SelectedDate
    {
      get { return selectedDate; }
      private set { SetProperty(ref selectedDate, value); }
    }

    public bool IsRecurrent
    {
      get { return isRecurrent; }
      set { SetProperty(ref isRecurrent, value); }
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

    public byte[] ImageData
    {
      get { return imageData; }
      private set { SetProperty(ref imageData, value); }
    }

    public DateTime Date
    {
      get { return date; }
      private set { SetProperty(ref date, value); }
    }

    public Time Time
    {
      get { return time; }
      private set { SetProperty(ref time, value); }
    }

    public int DaysRemaining
    {
      get { return daysRemaining; }
      private set { SetProperty(ref daysRemaining, value); }
    }

    public int DaysTotal
    {
      get { return daysTotal; }
      private set { SetProperty(ref daysTotal, value); }
    }

    public double ProgressPercentage
    {
      get { return progressPercentage; }
      private set { SetProperty(ref progressPercentage, value); }
    }

    public DateTime ExpirationDate
    {
      get { return expirationDate; }
      private set { SetProperty(ref expirationDate, value); }
    }

    public int? RepetitionCount
    {
      get { return repetitionCount; }
      private set { SetProperty(ref repetitionCount, value); }
    }

    public bool Expired
    {
      get { return expired; }
      private set { SetProperty(ref expired, value); }
    }

    public bool TimeToggled
    {
      get { return timeToggled; }
      private set
      {
        if (SetProperty(ref timeToggled, value))
          Update();
      }
    }

    public ICommand ToggleTimeCommand
    {
      get { return toggleTimeCommand ?? (toggleTimeCommand = new Command(ToggleTime)); }
    }

    public static CountdownViewModel Create(Countdown countdown)
    {
      var viewModel = new CountdownViewModel();
      viewModel.Refresh(countdown);

      return viewModel;
    }

    public void Refresh(Countdown countdown)
    {
      this.countdown = countdown;

      Name = countdown.Name;
      SelectedDate = countdown.Date;
      IsRecurrent = countdown.IsRecurrent;
      FinishedNotification = countdown.FinishedNotification;
      PinnedToStart = countdown.PinnedToStart;
      ImageData = countdown.ImageData;

      Update();
    }

    public void Update()
    {
      var now = DateTime.Now;

      var schedule = CountdownCalculator.GetSchedule(countdown, now);
      var progress = CountdownCalculator.GetProgress(schedule, now);

      if (timeToggled)
      {
        Time = CountdownCalculator.GetElapsedTime(schedule, now);
        Date = schedule.Started;
      }
      else
      {
        Time = CountdownCalculator.GetRemainingTime(schedule, now);
        Date = schedule.NextCycle;
      }

      ExpirationDate = schedule.NextCycle;
      RepetitionCount = schedule.NumberOfCycles;
      Expired = schedule.Expired;
      DaysRemaining = progress.DaysRemaining;
      DaysTotal = progress.DaysTotal;
      ProgressPercentage = progress.ProgressPercentage;
    }

    private void ToggleTime()
    {
      TimeToggled = !TimeToggled;
    }
  }
}