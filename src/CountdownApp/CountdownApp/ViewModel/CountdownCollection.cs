using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CountdownApp
{
  public class CountdownCollection
  {
    private readonly ObservableCollection<CountdownViewModel> items = new ObservableCollection<CountdownViewModel>();

    public ObservableCollection<CountdownViewModel> Items
    {
      get { return items; }
    }

    public bool IsEmpty
    {
      get { return items.Count == 0; }
    }

    public void Initialize(IEnumerable<Countdown> countdowns)
    {
      var viewModels = countdowns.Select(CountdownViewModel.Create).ToList();
      viewModels.Sort(CompareCountdowns);

      items.Clear();

      foreach (var viewModel in viewModels)
      {
        items.Add(viewModel);
      }
    }

    public void UpdateCountdowns()
    {
      foreach (var countdown in items)
      {
        countdown.Update();
      }
    }

    public CountdownViewModel RefreshCountdown(Countdown countdown)
    {
      var viewModel = items.SingleOrDefault(x => x.CountdownId == countdown.Id) ?? new CountdownViewModel();
      viewModel.Refresh(countdown);

      InsertOrMoveCountdown(viewModel);

      return viewModel;
    }

    private void InsertOrMoveCountdown(CountdownViewModel viewModel)
    {
      var index = 0;

      var otherCountdowns = items.Where(x => x.CountdownId != viewModel.CountdownId).ToList();
      if (otherCountdowns.Count > 0)
      {
        index = otherCountdowns.FindIndex(x => CompareCountdowns(viewModel, x) < 0);
        if (index < 0)
          index = otherCountdowns.Count;
      }

      var added = !items.Contains(viewModel);
      if (added)
      {
        items.Insert(index, viewModel);
      }
      else
      {
        var currentIndex = items.IndexOf(viewModel);
        if (currentIndex != index)
          items.Move(currentIndex, index);
      }
    }

    public void RemoveCountdown(CountdownViewModel countdown, out CountdownViewModel nextCountdown)
    {
      var index = items.IndexOf(countdown);

      items.RemoveAt(index);

      if (items.Count > 0)
      {
        if (index >= items.Count)
          index = items.Count - 1;

        nextCountdown = items[index];
      }
      else
      {
        nextCountdown = null;
      }
    }

    public CountdownViewModel FindCountdown(string guid)
    {
      return items.SingleOrDefault(x => x.Guid == guid);
    }

    public CountdownViewModel NextCountdown(CountdownViewModel countdown)
    {
      var index = items.IndexOf(countdown) + 1;
      if (index < items.Count)
        return items[index];

      return null;
    }

    public CountdownViewModel PreviousCountdown(CountdownViewModel countdown)
    {
      var index = items.IndexOf(countdown) - 1;
      if (index >= 0)
        return items[index];

      return null;
    }

    private static int CompareCountdowns(CountdownViewModel x, CountdownViewModel y)
    {
      var days1 = x.DaysRemaining;
      var days2 = y.DaysRemaining;

      if (days1 >= 0 && days2 >= 0)
      {
        var result = days1.CompareTo(days2);
        if (result != 0)
          return result;
      }
      else if (days1 >= 0)
      {
        return -1;
      }
      else if (days2 >= 0)
      {
        return 1;
      }
      else
      {
        var result = days2.CompareTo(days1);
        if (result != 0)
          return result;
      }

      var result2 = x.ExpirationDate.CompareTo(y.ExpirationDate);
      if (result2 != 0)
        return result2;

      result2 = string.Compare(x.Name, y.Name, StringComparison.CurrentCulture);
      if (result2 != 0)
        return result2;

      return x.CountdownId.CompareTo(y.CountdownId);
    }
  }
}